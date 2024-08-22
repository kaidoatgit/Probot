using DSharpPlus.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using ProPayments.Client.Configs;
using ProPayments.Client.Dtos.Order.Response;
using ProPayments.Client.Dtos.Subscription.Response;
using ProPayments.Client.Extensions;
using ProPayments.Client.Helpers;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Services.Managers
{
    public class HubManager
    {
        private AppSettings _appSettings;
        private readonly HubConnection _hubConnection;
        private readonly UserManager _userManager;
        private readonly OrderManager _orderManager;
        private readonly Mapper _mapper;
        public HubManager(IOptionsMonitor<AppSettings> appSettings, UserManager userManager, OrderManager orderManager, Mapper mapper)
        {
            _hubConnection = new HubConnectionBuilder()
                       .WithUrl("https://localhost:7240/notificationHub")
                       .WithAutomaticReconnect()
                       .Build();

            appSettings.OnChange(updatedSettings =>
            {
                _appSettings = updatedSettings;
                Console.WriteLine("AppSettings changed!");
            });
            _appSettings = appSettings.CurrentValue;

            _userManager = userManager;
            _orderManager = orderManager;
            _mapper = mapper;

        }

        public async Task StartAsync()
        {
            await _hubConnection.StartAsync();
            Console.WriteLine("Hub connection started.");
        }

        public async Task StopAsync()
        {
            await _hubConnection.StopAsync();
            Console.WriteLine("Hub connection stopped.");
        }

        public async Task DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        public void RegisterHandlers(DiscordGuild guild)
        {
            RegisterHandler<OrderResult>("ReceiveOrderResult", async (orderResult) => await HandleReceiveOrderResult(guild, orderResult));
            RegisterHandler<SubscriptionReminder>("ReceiveSubscriptionReminder", async (subscriptionReminder) => await HandleSubscriptionReminder(guild, subscriptionReminder));
        }

        private void RegisterHandler<T>(string methodName, Action<T> handler)
        {
            _hubConnection.On(methodName, handler);
        }

        private void RegisterHandler<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            _hubConnection.On(methodName, handler);
        }

        private async Task HandleReceiveOrderResult(DiscordGuild guild, OrderResult orderResult)
        {
            var order = _orderManager.GetOrder(orderResult.OrderId);
            try
            {
                var orderInteraction = order!.Interaction!;
                var discordInteraction = orderInteraction.DiscordInteraction;

                switch (orderResult.OrderStatus)
                {
                    case OrderStatus.Expired:
                        {
                            await discordInteraction.DeleteFollowupMessageAsync(orderInteraction.MessageId);
                            Console.WriteLine($"[Expired] Order: {order.Id}");
                            break;
                        }
                    case OrderStatus.DbError:
                        {
                            //informar ao utilizador que a transação foi confirmada mas existiu erro na DB
                            await discordInteraction.DeleteFollowupMessageAsync(orderInteraction.MessageId);
                            Console.WriteLine($"[DbError] Order: {order.Id}");
                            break;
                        }
                    case OrderStatus.Completed:
                        {
                            ulong userId = orderResult.UserId;
                            int totalProductKeys = orderResult.TotalProductKeys;
                            var guildRoles = guild.Roles;
                            DiscordMember member = (DiscordMember)discordInteraction.User;

                            await discordInteraction.NotifyWithPaidSubscription(orderInteraction.MessageId, _appSettings.BotChannelId, totalProductKeys);
                            await guild.NotifyOnSubscriptionAlertChannel(_appSettings.NotificationChannelId, userId, totalProductKeys);
                            Console.WriteLine($"Total product roles: {orderResult.ProductRoleIds.Count}");
                            await member.AddRolesAsync(guildRoles, orderResult.ProductRoleIds);
                            Console.WriteLine($"[Completed] Order: {order.Id}");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][HandleReceiveOrderResult] {ex.Message}");
            }
            finally
            {
                if(order != null)
                {
                    _orderManager.RemoveOrder(order);
                }
            }
        }

        private async Task HandleSubscriptionReminder(DiscordGuild guild, SubscriptionReminder subscriptionReminder)
        {
            Console.WriteLine(subscriptionReminder.ToString());
            try
            {
                var channel = guild.GetChannel(_appSettings.NotificationChannelId);
                var member = await guild.GetMemberAsync(subscriptionReminder.UserId);
                var mention = new UserMention(member);

                if (subscriptionReminder.IsSubscriptionActive)
                {
                    var message = new DiscordMessageBuilder()
                        .AddEmbed(EmbedHelper.CreateSubscriptionEndingSoonEmbed(subscriptionReminder.SubscriptionEndDate, subscriptionReminder.DaysLeft))
                        .WithAllowedMention(mention)
                        .WithContent($"||{member.Mention}||");
                    await channel.SendMessageAsync(message);
                }
                else
                {
                    var role = guild.GetRole(subscriptionReminder.ProductRoleId);
                    if (subscriptionReminder.IsToNotifyUser)
                    {
                        var message = new DiscordMessageBuilder()
                            .AddEmbed(EmbedHelper.CreateSubscriptionOverEmbed())
                            .WithAllowedMention(mention)
                            .WithContent($"||{member.Mention}||");
                        await channel.SendMessageAsync(message);
                        await member.RevokeRoleAsync(role);
                    }
                    else
                    {
                        await member.RevokeRoleAsync(role);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][HandleSubscriptionReminder][{subscriptionReminder.Username}] {ex.Message}");
            }
            finally
            {
                if (!subscriptionReminder.IsSubscriptionActive)
                {
                    _userManager.RemoveSubscriptionForUser(subscriptionReminder.UserId, subscriptionReminder.ProductRoleId);
                }
            }
        }

    }
}
