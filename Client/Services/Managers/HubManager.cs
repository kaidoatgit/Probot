using System.Text;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Configs;
using ProPayments.Client.Dtos.Order.Response;
using ProPayments.Client.Dtos.Subscription.Response;
using ProPayments.Client.Extensions;
using ProPayments.Client.Helpers;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Services.Managers
{
    public partial class HubManager
    {
        private AppSettings _appSettings;
        private readonly HubConnection _hubConnection;
        private readonly OrderManager _orderManager;
        private readonly UserManager _userManager;
        private readonly ProductManager _productManager;
        public HubManager(IOptionsMonitor<AppSettings> appSettings, OrderManager orderManager, UserManager userManager, ProductManager productManager)
        {
            _hubConnection = new HubConnectionBuilder()
                       .WithUrl("https://localhost:7240/notificationhub")
                       .WithAutomaticReconnect()
                       .Build();
            _hubConnection.Reconnecting += error =>
            {
                Console.WriteLine($"Reconnecting due to: {error?.Message}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                Console.WriteLine($"Reconnected successfully with connectionId {connectionId}");
                // Consider resubscribing to any channels or sending a message to confirm the connection.
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                Console.WriteLine($"Connection closed due to: {error?.Message}. Restarting connection...");
                return Task.CompletedTask;
            };

            appSettings.OnChange(updatedSettings =>
            {
                _appSettings = updatedSettings;
                Console.WriteLine("AppSettings changed!");
            });
            _appSettings = appSettings.CurrentValue;

            _orderManager = orderManager;
            _userManager = userManager;
            _productManager = productManager;
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

            RegisterHandler<Dictionary<ulong, List<SubscriptionReminder>>>("ReceiveSubscriptionsReminders", async (userSubscriptionsReminders) 
                => await HandleSubscriptionsReminders(guild, userSubscriptionsReminders));
                
            RegisterHandler<HashSet<User>>("ReceiveUsers", async (users) => await UpdateUsersRoles(guild, users));
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
                            // ulong userId = orderResult.UserId;
                            // int totalProductKeys = orderResult.TotalProductKeys;
                            // var guildRoles = guild.Roles;
                            // DiscordMember member = (DiscordMember)discordInteraction.User;

                            // await discordInteraction.NotifyWithPaidSubscription(orderInteraction.MessageId, _appSettings.BotChannelId, totalProductKeys);
                            // await guild.NotifyOnSubscriptionAlertChannel(_appSettings.NotificationChannelId, userId, totalProductKeys);
                            // await member.AddRolesAsync(guildRoles, orderResult.ProductRoleIds);
                            // _userManager.AddOrUpdateSubscriptionForUser(orderResult.UserId, orderResult.ProductRoleIds);
                            // Console.WriteLine($"[Completed] Order: {order.Id}");
                            ulong orderUserId = orderResult.UserId;
                            int totalKeysCount  = orderResult.TotalKeysByProduct.Values.Sum();
                            List<ulong> productRoles = orderResult.TotalKeysByProduct.Keys.ToList();
                            DiscordMember member = (DiscordMember)discordInteraction.User;

                            await discordInteraction.NotifyWithPaidSubscription(orderInteraction.MessageId, _appSettings.BotChannelId, totalKeysCount);
                            await guild.NotifyOnSubscriptionAlertChannel(_appSettings.NotificationChannelId, orderUserId, totalKeysCount);
                            await member.AddRolesAsync(guild.Roles, productRoles);
                            _userManager.AddInactivatedKeysPerProduct(orderUserId, orderResult.TotalKeysByProduct);
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

        private async Task HandleSubscriptionsReminders(DiscordGuild guild, Dictionary<ulong, List<SubscriptionReminder>> userSubscriptionsReminders)
        {
            Console.WriteLine($"Total groups: {userSubscriptionsReminders.Count}");
            foreach (var kvp in userSubscriptionsReminders)
            {
                var userId = kvp.Key; 
                var subsReminders = kvp.Value;
                var member = await guild.GetMemberAsync(userId);
                StringBuilder description = new();
                Console.WriteLine($"Group for UserId: {userId}, SubsReminders count: {subsReminders.Count}");

                foreach (var subReminder in subsReminders)
                {
                    Console.WriteLine($"SubsReminders count: {subsReminders.Count}");
                    //dont notify subscriptions which ended over past 3 days
                    if (!subReminder.IsActive && DateTime.UtcNow.AddDays(-3) > subReminder.EndDate)
                    {
                        continue;
                    }
                    
                    description.AppendLine();
                    description.Append($"{EmojisHelper.Key} Alphabot Key");
                    // description.Append($"```{subReminder.AlphabotKey}```");
                    if (subReminder.IsActive)
                    {
                        description.Append($" | Expire on <t:{subReminder.EndDate.ToUnixTimeSeconds()}:D>");
                        description.Append($" | Days left: **{subReminder.DaysLeft}**");
                        // description.AppendLine($"Your subscription will expire on <t:{subReminder.EndDate.ToUnixTimeSeconds()}:D>");
                        // description.AppendLine($"You have **{subReminder.DaysLeft}** days left to enjoy all your benefits.");
                    }
                    else
                    {
                        description.Append($" | Expired on <t:{subReminder.EndDate.ToUnixTimeSeconds()}:D>");
                        // description.AppendLine($"Your subscription expired on <t:{subReminder.EndDate.ToUnixTimeSeconds()}:D>");
                        // description.AppendLine("**Renew now** to regain all your awesome benefits and continue enjoying our services!");
                        // _userManager.RemoveSubscriptionForUser(subReminder.UserId, subReminder.ProductRoleId);
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
                // await member.RevokeRolesAsync(guild.Roles, userSubscription.TotalActiveSubsByProduct);
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
          
        private async Task UpdateUsersRoles(DiscordGuild guild, HashSet<User> users)
        {
            foreach (var user in users)
            {
                var userId = user.Id;
                var member = await guild.GetMemberAsync(userId);
                var productsInactived = new HashSet<ulong>();

                var inactiveKeysPerProduct = user.InactiveKeysPerProduct;
                foreach (var (product, inactiveKeysCount) in inactiveKeysPerProduct)
                {
                    if(inactiveKeysCount > 0)
                    {
                        productsInactived.Add(product);
                    }
                }

                var activeSubsPerProduct = user.ActiveSubsPerProduct;
                foreach (var (product, activeSubsCount) in activeSubsPerProduct)
                {
                    if(activeSubsCount > 0)
                    {
                        // Add role if active subscriptions exist
                        await member.TryAddRoleAsync(guild.Roles, product);
                    }
                    else if(!productsInactived.Contains(product))
                    {
                        // Revoke role only if it's not in the inactive list
                        await member.TryRevokeRoleAsync(guild.Roles, product);
                    }
                }

                // If no active subscriptions or inactive keys, revoke all product roles
                if(inactiveKeysPerProduct.Count == 0 && activeSubsPerProduct.Count == 0)
                {
                    await member.RevokeRolesAsync(guild.Roles, _productManager.Products.Select(p => p.RoleId).ToList());
                }
            }
        }
    }
}
