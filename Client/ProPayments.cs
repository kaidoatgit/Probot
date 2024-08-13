using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProPayments.Client.Commands;
using ProPayments.Client.Configs;
using ProPayments.Client.Exceptions;
using ProPayments.Client.Extensions;
using ProPayments.Client.Helpers;
using ProPayments.Client.Models;
using ProPayments.Client.Services.Managers;
using ProPayments.Client.Services.Services;

namespace ProPayments.Client
{
    public class ProPayments
    {
        private readonly DiscordClient _discordClient;
        private readonly PlanManager _planManager;
        private readonly UserManager _userManager;
        private readonly OrderManager _orderManager;
        private readonly HubManager _hubManager;
        private readonly SubscriptionService _subscriptionService;
        private readonly CancelationTokenManager _tokenManager;
        private readonly AppSettings _appSettings;
        private readonly DiscordManager _discordManager;
        private readonly CartManager _cartManager;

        public SlashCommandsExtension? SlashCommands { get; private set; }

        public ProPayments(IOptions<AppSettings> appSettings, PlanManager planManager, UserManager userManager, OrderManager orderManager, SubscriptionService subscriptionService, HubManager hubManager, DiscordManager discordManager, CancelationTokenManager tokenManager, CartManager cartManager)
        {
            _appSettings = appSettings.Value;
            _discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = _appSettings.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Error,
                AutoReconnect = false
            });

            _planManager = planManager;
            _orderManager = orderManager;
            _userManager = userManager;
            _subscriptionService = subscriptionService;
            _hubManager = hubManager;
            _discordManager = discordManager;
            _tokenManager = tokenManager;
            _cartManager = cartManager;

            Console.WriteLine("Probot created");
        }

        public async Task RunAsync(IServiceProvider services)
        {
            _discordClient.UseInteractivity(new InteractivityConfiguration()
            {
               Timeout = TimeSpan.FromSeconds(60)
            });
            _discordClient.Ready += OnClientReady;
            _discordClient.ClientErrored += OnClientErrored;
            _discordClient.ComponentInteractionCreated += OnClientComponentInteractionCreated;
            _discordClient.ModalSubmitted += OnClientModalSubmitted;
            _discordClient.GuildAvailable += OnGuildAvailable;
            _discordClient.GuildMemberUpdated += OnGuildMemberUpdated;

            var slash = _discordClient.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = services
            });

            slash.RegisterCommands<UserCommands>();

            await _discordClient.ConnectAsync();

            try
            {
                await Task.Delay(-1, _tokenManager.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Discord bot terminating");
            }
            finally
            {
                _discordClient!.Ready -= OnClientReady;
                _discordClient!.ClientErrored -= OnClientErrored;
                _discordClient!.ComponentInteractionCreated -= OnClientComponentInteractionCreated;
                _discordClient!.ModalSubmitted -= OnClientModalSubmitted;
                _discordClient!.GuildAvailable -= OnGuildAvailable;
                _discordClient.GuildMemberUpdated -= OnGuildMemberUpdated;
                await _hubManager.StopAsync();
                await _discordClient.DisconnectAsync();
                await _hubManager.DisposeAsync();
                _tokenManager.Dispose();
                _discordClient.Dispose();
            }
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            Console.WriteLine("[Event] client ready fired [Event]");
            return Task.CompletedTask;
        }

        private async Task OnGuildAvailable(DiscordClient sender, GuildCreateEventArgs args)
        {
            Console.WriteLine("[Event] guild available fired [Event]");

            var guildRoles = args.Guild.Roles.Values.ToList();
            bool plansLoadedSuccessfully = await _planManager.LoadPlansAsync(guildRoles);
            if (!plansLoadedSuccessfully)
            {
                _tokenManager.Cancel();
                return;
            }
            Console.WriteLine($"Total plans: {_planManager.Plans.Count}");

            bool usersLoadedSuccessfully = await _userManager.LoadUsersToMemoryAsync();
            if (!usersLoadedSuccessfully)
            {
                _tokenManager.Cancel();
                return;
            }
            Console.WriteLine($"Total users: {_userManager.Users.Count}");

            bool subscriptionProcessAddedSuccessfully = await args.Guild.AddSubscriptionProcess(_appSettings.SubscriptionChannelId);
            if (!subscriptionProcessAddedSuccessfully)
            {
                _tokenManager.Cancel();
                return;
            }

            bool commandsAddedSuccessfully = await args.Guild.AddAlphabotCommandsInfo(_appSettings.BotChannelId);
            if (!commandsAddedSuccessfully)
            {
                _tokenManager.Cancel();
                return;
            }
            
            _hubManager.RegisterHandlers(args.Guild);
            await _hubManager.StartAsync();
        }

        private Task OnClientErrored(DiscordClient sender, ClientErrorEventArgs args)
        {
            Console.WriteLine($"Event: {args.EventName}, Exception: {args.Exception}");
            return Task.CompletedTask;
        }

        private async Task OnGuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            Console.WriteLine("[Event] guild member update fired [Event]");
            var discordMember = e.Member;
            if (discordMember.IsBot)
            {
                Console.WriteLine($"{e.Member.Username}");
                return;
            }

            var roles = e.Guild.Roles;
            var verifiedRole = roles.Values.FirstOrDefault(role => role.Name == "Verified");
            if (verifiedRole == null) return;

            // Only proceed if the verified role was newly added
            if (!e.RolesBefore.Contains(verifiedRole) && e.RolesAfter.Contains(verifiedRole))
            {
                var user = _userManager.GetUserFromMemory(discordMember.Id);
                await _discordManager.UpdateDiscordMemberAsync(discordMember, roles, user?.Subscriptions);
            }
        }

        private async Task OnClientComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            DiscordMessage discordMessage = args.Message;
            DiscordUser discordUser = args.User;
            switch (args.Id)
            {
                case "subscribe_btn":
                    {
                        var user = _userManager.GetUserFromMemory(discordUser.Id);
                        if (user == null || string.IsNullOrWhiteSpace(user.WalletAddress))
                        {
                            await args.Interaction.NotifyWithMessage(MessageHelper.PaymentWalletNotFoundMessage, deleteMsg: true, after: TimeSpan.FromSeconds(5));
                        }
                        else
                        {
                            await args.Interaction.NotifyWithSubscribeOptions(_planManager.Plans);

                            var responseMessage = await args.Interaction.GetOriginalResponseAsync();
                            ulong messageId = responseMessage.Id;
                            _cartManager.InitCart(messageId);
                        }
                        break;
                    }

                case "wallet_btn":
                    {
                        await args.Interaction.NotifyWithWalletModal();
                        break;
                    }

                case "plan_details_btn":
                    {
                        await args.Interaction.NotifyWithPlanDetails(_planManager.Plans);
                        break;
                    }

                case "product_selection_menu":
                    {
                        var selectedPlanId = args.Values.First();

                        var durationDropdown = GetDurationsPricesBasedOnPlanSelected(selectedPlanId);
                        var components = discordMessage.SetDropdownDefaultValue(args.Id, selectedPlanId, durationDropdown);
                        var planMessageBuilder = discordMessage.ReplaceComponents(components);

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                            new DiscordInteractionResponseBuilder(planMessageBuilder));
                        break;
                    }

                case "duration_selection_menu":
                    {
                        var selectedDurationPrice = args.Values.First();

                        var components = discordMessage.SetDropdownDefaultValue(args.Id, selectedDurationPrice);
                        var durationMessageBuilder = discordMessage.ReplaceComponents(components);

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                            new DiscordInteractionResponseBuilder(durationMessageBuilder));
                        break;
                    }

                case "add_item_cart_btn":
                {
                    var (planSelect, durationSelect) = discordMessage.ParseComponentSelections();
                    var selectedPlan = planSelect?.Options.FirstOrDefault(o => o.Default);
                    var selectedDuration = durationSelect?.Options.FirstOrDefault(o => o.Default);

                    if (selectedPlan == null || selectedDuration == null)
                    {
                        await args.Interaction.NotifyWithMessage(MessageHelper.MissingProductOrDuration, deleteMsg: true, after: TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        Console.WriteLine($"{selectedPlan.Label}|{selectedDuration.Label}");

                        Plan plan = _planManager.GetPlanByRoleId(selectedPlan.Value)!;
                        int selectedPeriod = int.Parse(selectedDuration.Value);
                        CartItem cartItem = new()
                        { 
                            ItemId = _cartManager.GetMaxCartItemId(discordMessage.Id) + 1,
                            SelectedPlan = selectedPlan.Label,
                            SelectedDuration = selectedDuration.Label,
                            Price = plan.GetPrice(selectedPeriod),
                            PlanOptionId = plan.GetPlanOptionId(selectedPeriod)
                        };
                        _cartManager.AddItemToCart(discordMessage.Id, cartItem);

                        var originalComponents = discordMessage.Components;
                        var embed = EmbedHelper.CreateShoppingCartEmbed(_cartManager.GetItemsFromCart(discordMessage.Id)!);

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                            .AddEmbed(embed)
                            .AddComponents(originalComponents));
                    }
                    break;
                }

                case "remove_item_cart_btn":
                {
                    try
                    {
                        var modalCustomId = await args.Interaction.NotifyWithItemRemovalModal(args.Interaction.Id);
                        var interactivity = _discordClient.GetInteractivity();
                        var modal = await interactivity.WaitForModalAsync(modalCustomId, discordUser);
                        if(modal.TimedOut)
                        {
                            return;
                        }

                        await modal.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        Console.WriteLine($"Ja nao estou: discordMessage.id={discordMessage.Id}");

                        var inputValue = modal.Result.Values.Values.First().Trim();
                        if (int.TryParse(inputValue, out int productId))
                        {
                            var successfullyRemoved = _cartManager.RemoveItemFromCart(discordMessage.Id, productId);
                            if(successfullyRemoved)
                            {
                                var originalComponents = discordMessage.Components;
                                var embed = EmbedHelper.CreateShoppingCartEmbed(_cartManager.GetItemsFromCart(discordMessage.Id)!);

                                await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                                    .AddEmbed(embed)
                                    .AddComponents(originalComponents));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
                    } 
                    break;
                }

                case "confirm_cart_btn":
                    {
                        try
                        {
                            if(!_cartManager.GetCart(discordMessage.Id)!.CartItems.Any())
                            {
                                await args.Interaction.NotifyWithMessage(MessageHelper.CartIsEmpty, deleteMsg: true, after: TimeSpan.FromSeconds(5));
                                return;
                            }
                            Console.WriteLine($"Total carts: {_cartManager.ShoppingCarts.Count()}");

                            await args.Interaction.DeferAsync(true);
                            Cart cart = _cartManager.GetCart(discordMessage.Id)!;
                            var order = await _orderManager.CreateOrderAsync(discordUser.Id, cart);
                            order.Interaction = new(discordMessage.Id, args.Interaction);
                            _orderManager.AddOrder(order);
                            await args.Interaction.NotifyUserToSendPayment(order);
                            _cartManager.RemoveCart(discordMessage.Id);

                            Console.WriteLine($"Total carts: {_cartManager.ShoppingCarts.Count()}");
                        }
                        catch (OrderException)
                        {
                            await args.Interaction.NotifyWithServerError(discordMessage.Id);
                        }
                        catch (Exception ex)
                        {
                            await args.Interaction.NotifyWithServerError(discordMessage.Id);
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    }
            }
        }

        private async Task OnClientModalSubmitted(DiscordClient sender, ModalSubmitEventArgs args)
        {
            if (args.Interaction.Type == InteractionType.ModalSubmit)
            {
                switch (args.Interaction.Data.CustomId)
                {
                    case "solana_submission_modal":
                    {
                        await args.Interaction.DeferAsync(true);
                        var userId = args.Interaction.User.Id;
                        var walletAddress = args.Values.Values.First().Trim();

                        var walletStatus = _userManager.GetWalletAddressStatus(userId, walletAddress);
                        switch (walletStatus.Result)
                        {
                            case Result.WalletExist:
                                {
                                    await args.Interaction.NotifyWithMessage(walletStatus.Message, defer: true, deleteMsg: true, after: TimeSpan.FromSeconds(5));
                                    return;
                                }
                            case Result.WalletFoundInOrder:
                                {
                                    await args.Interaction.NotifyWithMessage(walletStatus.Message, defer: true, deleteMsg: true, after: TimeSpan.FromSeconds(10));
                                    return;
                                }
                        }

                        var username = args.Interaction.User.Username;
                        var isResultSuccess = await _userManager.AddOrUpdateUserAsync(userId, username, walletAddress);
                        if (isResultSuccess)
                        {
                            await args.Interaction.NotifyWithMessage(MessageHelper.WalletSubmitSuccess(walletAddress), defer: true);
                        }
                        else
                        {
                            await args.Interaction.NotifyWithMessage(MessageHelper.GenericErrorMessage(), defer: true);
                        }
                        break;
                    }

                    // case "cart_item_removal_submission_modal":
                    // {
                    //     Console.WriteLine($"int: {args.Interaction.Id}");

                    //     var inputValue = args.Values.Values.First().Trim();
                    //     if (int.TryParse(inputValue, out int productId))
                    //     {
                    //         var responseMessage = await args.Interaction.GetOriginalResponseAsync();
                    //         ulong messageId = responseMessage.Id;
                    //         _cartManager.RemoveItemFromCart(args.Interaction.Id, productId);
                    //         // if (result)
                    //         // {
                    //         //     await args.NotifyWithMessage($"Item {productId} has been removed from your cart.", defer: true);
                    //         // }
                    //         // else
                    //         // {
                    //         //     await args.NotifyWithMessage($"Failed to remove item {productId} from your cart. Please try again.", defer: true);
                    //         // }
                    //     }
                    //     await args.Interaction.NotifyWithMessage("Just testing", deleteMsg: true, after: TimeSpan.FromSeconds(5));
                    //     break;
                    // }
                }
            }
        }

        public void Stop()
        {
            try
            {
                _tokenManager.Cancel();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception during shutdown: {e}");
            }
        }

        

        public DiscordSelectComponent GetDurationsPricesBasedOnPlanSelected(string selectedPlan)
        {
            var durationOptions = _planManager
                        .GetDurationsWithPrices(selectedPlan)!
                        .Select(option => new DiscordSelectComponentOption(option.PeriodDescription, option.Period.ToString()))
                        .AsEnumerable();
            return new DiscordSelectComponent("duration_selection_menu", "Month(s) subscription", durationOptions);
        }
    }
}


