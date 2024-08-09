using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProPayments.Client.Commands;
using ProPayments.Client.Configs;
using ProPayments.Client.Exceptions;
using ProPayments.Client.Extensions;
using ProPayments.Client.Helpers;
using ProPayments.Client.Models;
using ProPayments.Client.Models.Enums;
using ProPayments.Client.Services.Managers;
using ProPayments.Client.Services.Services;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Channels;

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

        public SlashCommandsExtension? SlashCommands { get; private set; }

        public ProPayments(IOptions<AppSettings> appSettings, PlanManager planManager, UserManager userManager, OrderManager orderManager, SubscriptionService subscriptionService, HubManager hubManager, DiscordManager discordManager, CancelationTokenManager tokenManager)
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

            Console.WriteLine("Probot created");
        }

        public async Task RunAsync(IServiceProvider services)
        {
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
            switch (args.Id)
            {
                case "subscribe_btn":
                    {
                        var user = _userManager.GetUserFromMemory(args.User.Id);
                        if (user == null || string.IsNullOrWhiteSpace(user.WalletAddress))
                        {
                            await args.Interaction.NotifyWithMessage(MessageHelper.PaymentWalletNotFoundMessage, deleteMsg: true, after: TimeSpan.FromSeconds(5));
                        }
                        else
                        {
                            await args.Interaction.NotifyWithSubscribeOptions(_planManager.Plans);
                        }
                        break;
                    }

                case "wallet_submission_btn":
                    {
                        await args.Interaction.NotifyWithWalletModal();
                        break;
                    }

                case "plan_details_btn":
                    {
                        await args.Interaction.NotifyWithPlanDetails(_planManager.Plans);
                        break;
                    }

                case "plan_selection":
                    {
                        var selectedPlanId = args.Values.First();

                        var durationDropdown = ComponentHelper.GetDurationsPricesBasedOnPlanSelected(_planManager, selectedPlanId);
                        var components = args.Message.SetDropdownDefaultValue(args.Id, selectedPlanId, durationDropdown);
                        var planMessageBuilder = args.Message.ReplaceComponents(components);

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                            new DiscordInteractionResponseBuilder(planMessageBuilder));
                        break;
                    }

                case "duration_selection":
                    {
                        var selectedDurationPrice = args.Values.First();

                        var components = args.Message.SetDropdownDefaultValue(args.Id, selectedDurationPrice);
                        var durationMessageBuilder = args.Message.ReplaceComponents(components);

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                            new DiscordInteractionResponseBuilder(durationMessageBuilder));
                        break;
                    }

                case "confirm_subscription_btn":
                    {
                        var (planSelect, durationSelect) = args.Message.ParseComponentSelections();
                        var selectedPlan = planSelect?.Options.FirstOrDefault(o => o.Default);
                        var selectedDuration = durationSelect?.Options.FirstOrDefault(o => o.Default);

                        if (selectedPlan == null || selectedDuration == null)
                        {
                            await args.Interaction.NotifyWithMessage(MessageHelper.MissingPlanOrDuration, deleteMsg: true, after: TimeSpan.FromSeconds(5));
                        }
                        else
                        {
                            try
                            {
                                Console.WriteLine($"{selectedPlan.Label}|{selectedDuration.Label}");
                                await args.Interaction.DeferAsync(true);

                                int selectedPeriod = int.Parse(selectedDuration.Value);
                                Plan plan = _planManager.GetPlanByRoleId(selectedPlan.Value)!;
                                DiscordUser user = args.User;
                        
                                if (plan.Type == PlanType.Free)
                                {
                                    var subscription = await _subscriptionService.CreateSubscriptionAsync(user.Id, plan, selectedPeriod);
                                    await args.Interaction.NotifyWithFreeSubscription(args.Message.Id, subscription);
                                    await args.Guild.GrantRoleAsync(user, subscription.PlanRoleId);
                                }
                                else
                                {
                                    var order = await _orderManager.CreateOrderAsync(user.Id, plan, selectedPeriod);
                                    order.Interaction = new(args.Message.Id, args.Interaction);
                                    _orderManager.AddOrder(order);
                                    await args.Interaction.NotifyUserToSendPayment(order);                                    
                                }
                            }
                            catch (SubscriptionException ex)
                            {
                                if (ex.StatusCodes == StatusCodes.Status409Conflict)
                                {
                                    await args.Interaction.NotifyWithFreePlanUsed(args.Message.Id);
                                }
                                else
                                {
                                    await args.Interaction.NotifyWithServerError(args.Message.Id);
                                }
                            }
                            catch (OrderException)
                            {
                                await args.Interaction.NotifyWithServerError(args.Message.Id);
                            }
                            catch (Exception ex)
                            {
                                await args.Interaction.NotifyWithServerError(args.Message.Id);
                                Console.WriteLine(ex.Message);
                            }
                        }
                        break;
                    }
            }
        }

        private async Task OnClientModalSubmitted(DiscordClient sender, ModalSubmitEventArgs args)
        {
            if (args.Interaction.Type == InteractionType.ModalSubmit)
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
    }
}


