using System.Text;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Probot.Client.Configs;
using Probot.Client.Extensions;
using Probot.Client.Helpers;
using Probot.Client.Models;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Dtos.Subscription.Response;
using Probot.Shared.Enums;

namespace Probot.Client.Services.Managers
{
    public partial class HubManager
    {
        private AppSettings _appSettings;
        private readonly HubConnection _hubConnection;
        private readonly OrderManager _orderManager;
        private readonly ProductManager _productManager;
        private readonly UserManager _userManager;
        public HubManager(IOptionsMonitor<AppSettings> appSettings, OrderManager orderManager, ProductManager productManager, UserManager userManager)
        {
            _hubConnection = new HubConnectionBuilder()
                       .WithUrl("https://localhost:7240/notificationhub")
                       .WithAutomaticReconnect()
                       .AddJsonProtocol(options => 
                            options.PayloadSerializerOptions = new()
                            {
                                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                                PropertyNameCaseInsensitive = true,
                            })
                        // .ConfigureLogging(logging =>
                        // {
                        //     logging.AddConsole();
                        //     logging.AddDebug();
                        // })
                       .Build();
            _hubConnection.Reconnecting += error =>
            {
                Console.WriteLine($"Reconnecting due to: {error?.Message}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                Console.WriteLine($"Reconnected successfully with connectionId {connectionId}");
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                Console.WriteLine($"Connection closed due to: {error?.Message}.");
                return Task.CompletedTask;
            };

            appSettings.OnChange(updatedSettings =>
            {
                _appSettings = updatedSettings;
                Console.WriteLine("AppSettings changed!");
            });
            _appSettings = appSettings.CurrentValue;

            _orderManager = orderManager;
            _productManager = productManager;
            _userManager = userManager;
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
            RegisterHandler<OrderResult>("ReceiveOrderResult", async (orderResult) => await HandleOrderResultAsync(guild, orderResult));

            RegisterHandler<Dictionary<ulong, List<SubscriptionReminder>>>("ReceiveSubscriptionsReminders", async (userSubscriptionsReminders) 
                => await HandleSubscriptionsRemindersAsync(guild, userSubscriptionsReminders));

            RegisterHandler<List<User>>("ReceiveUsersMetrics", async (usersMetrics) => await ProcessUsersMetricsAsync(guild, usersMetrics));
        }

        private void RegisterHandler<T>(string methodName, Action<T> handler)
        {
            _hubConnection.On(methodName, handler);
        }

        private void RegisterHandler<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            _hubConnection.On(methodName, handler);
        }

        private async Task HandleOrderResultAsync(DiscordGuild guild, OrderResult orderResult)
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
                            ulong orderUserId = orderResult.UserId;
                            int totalKeysCount  = orderResult.PurchasedKeysCountPerProduct.Values.Sum();
                            List<ulong> productRoles = orderResult.PurchasedKeysCountPerProduct.Keys.ToList();
                            DiscordMember member = (DiscordMember)discordInteraction.User;

                            await discordInteraction.NotifyWithPaidSubscription(orderInteraction.MessageId, _appSettings.ProRaffleChannelId, totalKeysCount);
                            await guild.NotifyOnSubscriptionAlertChannel(_appSettings.NotificationChannelId, orderUserId, totalKeysCount);
                            await member.AddRolesAsync(guild.Roles, productRoles);
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

        private async Task HandleSubscriptionsRemindersAsync(DiscordGuild guild, Dictionary<ulong, List<SubscriptionReminder>> userSubscriptionsReminders)
        {
            Console.WriteLine($"Users to remind: {userSubscriptionsReminders.Count}");
            foreach (var (userId, subsReminders) in userSubscriptionsReminders)
            {
                var member = await guild.GetMemberAsync(userId);
                StringBuilder description = new();
                Console.WriteLine($"{userId}|{member.Nickname} - #Reminders: {subsReminders.Count}");

                foreach (var subReminder in subsReminders)
                {
                    // Skip subscriptions that ended more than 3 days ago
                    if (!subReminder.IsActive && DateTime.UtcNow.AddDays(-3) > subReminder.EndDate)
                    {
                        continue;
                    }
                    
                    description.AppendLine();
                    description.Append($"{EmojisHelper.Key} Alphabot Key");
                    if (subReminder.IsActive)
                    {
                        description.Append($" | Expire on <t:{subReminder.EndDate.ToUnixTimeSeconds()}:D>");
                        description.Append($" | Days left: **{subReminder.DaysLeft}**");
                    }
                    else
                    {
                        description.Append($" | Expired on <t:{subReminder.EndDate.ToUnixTimeSeconds()}:D>");
                    }
                    description.Append($"```{subReminder.AlphabotKey}```");
                }

                if(description.Length > 0)
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = $"{EmojisHelper.Bell} Subscriptions Reminder",
                        Description = description.ToString(),
                        Color = DiscordColor.Orange,
                        Footer = new() { Text = "Pro Payments" },
                        Timestamp = DateTime.UtcNow
                    };
                
                    var dmChannel = await member.CreateDmChannelAsync();
                    await dmChannel.SendMessageAsync(embed);
                }

                #region example of general notification chat
                // try
                // {
                    // var channel = guild.GetChannel(_appSettings.NotificationChannelId);
                    // ...
                    // var mention = new UserMention(member);
                    // ...
                    // if (subscriptionReminder.IsActive)
                    // {
                    //     var message = new DiscordMessageBuilder()
                    //         .AddEmbed(EmbedHelper.CreateSubscriptionEndingSoonEmbed(subscriptionReminder.SubscriptionEndDate, subscriptionReminder.DaysLeft))
                    //         .WithAllowedMention(mention)
                    //         .WithContent($"||{member.Mention}||");
                    //     await channel.SendMessageAsync(message);
                    // }
                    // else
                    // {
                    //     var role = guild.GetRole(subscriptionReminder.ProductRoleId);
                    //     if (subscriptionReminder.IsToNotifyUser)
                    //     {
                    //         var message = new DiscordMessageBuilder()
                    //             .AddEmbed(EmbedHelper.CreateSubscriptionOverEmbed())
                    //             .WithAllowedMention(mention)
                    //             .WithContent($"||{member.Mention}||");
                    //         await channel.SendMessageAsync(message);
                    //         await member.RevokeRoleAsync(role);
                    //     }
                    //     else
                    //     {
                    //         await member.RevokeRoleAsync(role);
                    //     }
                    // }
                // }
                // catch (Exception ex)
                // {
                //     Console.WriteLine($"[Error][HandleSubscriptionReminder]{ex.Message}");
                //     Console.WriteLine($"[Error][HandleSubscriptionReminder][{subscriptionReminder.Username}] {ex.Message}");
                // }
                // finally
                // {
                //     if (!subscriptionReminder.IsSubscriptionActive)
                //     {
                        // _userManager.RemoveSubscriptionForUser(subscriptionReminder.UserId, subscriptionReminder.ProductRoleId);
                //     }
                // }
                #endregion
                
            }
        }
        
        private async Task ProcessUsersMetricsAsync(DiscordGuild guild, List<User> usersMetrics)
        {
            Console.WriteLine($"[ProcessUsersMetricsAsync] Total users: {usersMetrics.Count}");
            foreach (var user in usersMetrics)
            {
                Metrics? metrics = user.Metrics;
                if(metrics == null) { continue; }

                DiscordMember? member = null!;
                try
                {
                    member = await guild.GetMemberAsync(user.Id);
                    _userManager.ReplaceMetricsForUser(user.Id, metrics);
                }
                catch (Exception)
                {
                    continue;
                }                

                var inactiveKeysPerProduct = metrics.InactiveKeysPerProduct;
                var activeSubsPerProduct = metrics.ActiveSubsPerProduct;

                // If no active subscriptions or inactive keys, revoke all product roles
                if(inactiveKeysPerProduct.Count == 0 && activeSubsPerProduct.Count == 0)
                {
                    await member.TryRevokeRolesAsync(guild.Roles, _productManager.Products.Select(p => p.RoleId).ToList());
                    continue;
                }

                foreach (var (roleId, role) in guild.Roles)
                {
                    var inactiveKeysCount = metrics.InactiveKeysPerProduct.GetValueOrDefault(roleId);
                    if(inactiveKeysCount > 0)
                    {
                        continue;
                    }

                    var activeSubsCount = metrics.ActiveSubsPerProduct.GetValueOrDefault(roleId);
                    if(activeSubsCount > 0)
                    {
                        continue;
                    }
                    await member.RevokeRoleAsync(role);
                }
            }
        }
    }
}
