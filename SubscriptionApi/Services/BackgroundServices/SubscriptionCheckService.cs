using System.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.Subscription.Response;
using Probot.SubscriptionApi.Services.Hubs;

namespace Probot.SubscriptionApi.Services.BackgroundServices
{
    public partial class SubscriptionCheckService : BackgroundService
    {
        #region Testing purpose
        // private static readonly TimeSpan _reminderPeriod = TimeSpan.FromSeconds(20);
        // // use now.AddHours(1) inside the method for simulating the time ticking
        // private DateTime now = DateTime.UtcNow; 
        #endregion

        private static readonly TimeSpan _reminderPeriod = TimeSpan.FromHours(1);
        private static readonly List<TimeSpan> _notificationPeriods = new()
        {
            TimeSpan.Zero,
            TimeSpan.FromDays(1),
            TimeSpan.FromDays(3),
            TimeSpan.FromDays(7)
        };

        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public SubscriptionCheckService(IServiceProvider serviceProvider, IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_reminderPeriod);
            var subsReminders = new List<SubscriptionReminder>();

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                var task = SendSubscriptionsRemindersAsync(stoppingToken)
                    .ContinueWith(async antecedentTask => 
                        await SendUsersMetricsAsync(stoppingToken, antecedentTask.Result), TaskContinuationOptions.OnlyOnRanToCompletion)
                    .Unwrap();
                
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SubscriptionCheckService] {ex.Message}");
                }
            }
        }

        private async Task<IEnumerable<ulong>> SendSubscriptionsRemindersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProbotContext>();
            var subscriptions = await dbContext.Subscriptions
                .Where(s => s.IsActive)
                .Include(s => s.ProductSetting)
                .Include(s => s.Product)
                .ToListAsync(stoppingToken);

            var subsReminders = new List<SubscriptionReminder>();
            var now = DateTime.UtcNow;

            foreach (var subscription in subscriptions)
            {
                TimeSpan timeLeft = subscription.EndDate - now;
                DateTimeOffset lastNotificationSent = subscription.LastNotificationCheck ?? DateTime.MinValue;

                foreach (var period in _notificationPeriods)
                {
                    DateTime notificationTime = subscription.EndDate - period;
                    if (timeLeft <= period && notificationTime > lastNotificationSent)
                    {    
                        if (subscription.ProductSetting is ProRaffleSetting proRaffleSettng) 
                        {
                            subscription.LastNotificationCheck = notificationTime;  
                            if(period == TimeSpan.Zero)
                            {
                                subscription.IsActive = false;
                                proRaffleSettng.IsPaused = true;
                            }   
                            try
                            {
                                await dbContext.SaveChangesAsync(stoppingToken);
                                subsReminders.Add(MapToSubscriptionReminder(subscription, proRaffleSettng.Key));
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                Console.WriteLine($"Concurrency exception: {ex.Message}");
                                dbContext.Entry(subscription).State = EntityState.Detached;
                                dbContext.Entry(proRaffleSettng).State = EntityState.Detached;
                            } 
                        }
                        break;
                    }
                }
            }

            if (subsReminders.Any())
            {
                Console.WriteLine("-------------------- -------------- ENTREI ReceiveSubscriptionsReminders ------------------ ----------------- ");
                Dictionary<ulong, List<SubscriptionReminder>> userSubscriptionsReminders = subsReminders
                    .GroupBy(s => s.UserId)
                    .ToDictionary(g => g.Key, g => g.ToList());
                await _hubContext.Clients.All.ReceiveSubscriptionsReminders(userSubscriptionsReminders);
                return subsReminders
                    .Where(reminder => !reminder.IsActive)
                    .Select(reminder => reminder.UserId)
                    .ToHashSet();
            }
            return Enumerable.Empty<ulong>();
        }

        
        #region Testing purpose
        // private async Task<IEnumerable<ulong>> SendSubscriptionsRemindersTestAsync(CancellationToken stoppingToken)
        // {
        //     var subsReminders = new List<SubscriptionReminder>();
        //     using var scope = _serviceProvider.CreateScope();
        //     var dbContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
        //     var subscriptions = await dbContext.Subscriptions
        //         .Where(s => s.IsActive && (s.Id == 2 || s.Id == 5 || s.Id == 8 || s.Id == 12))
        //         .Include(s => s.ProductSetting)
        //         .Include(s => s.Product)
        //         .ToListAsync(cancellationToken: stoppingToken);

        //     foreach(var subscription in subscriptions)
        //     {
        //         if (subscription.ProductSetting is ProRaffleSetting proRaffleSetting)
        //         {
        //             proRaffleSetting.IsPaused = true;
        //             subscription.IsActive = false;

        //             try
        //             {
        //                 await dbContext.SaveChangesAsync(stoppingToken);
        //                 subsReminders.Add(MapToSubscriptionReminder(subscription, proRaffleSetting.Key));
        //             }
        //             catch (DbUpdateConcurrencyException ex)
        //             {
        //                 Console.WriteLine($"Concurrency exception: {ex.Message}");
        //                 dbContext.Entry(subscription).State = EntityState.Detached;
        //                 dbContext.Entry(proRaffleSetting).State = EntityState.Detached;
        //             }   
        //         }
        //     }
        //     // subsReminders.Clear();
        //     if (subsReminders.Any())
        //     {
        //         Console.WriteLine("-------------------- -------------- ENTREI ReceiveSubscriptionsReminders ------------------ ----------------- ");
        //         Dictionary<ulong, List<SubscriptionReminder>> userSubscriptionsReminders = subsReminders
        //             .GroupBy(s => s.UserId)
        //             .ToDictionary(g => g.Key, g => g.ToList());
        //         await _hubContext.Clients.All.ReceiveSubscriptionsReminders(userSubscriptionsReminders);
        //         return subsReminders
        //             .Where(reminder => !reminder.IsActive)
        //             .Select(reminder => reminder.UserId)
        //             .ToHashSet();
        //     }
        //     return Enumerable.Empty<ulong>();
        // }
        #endregion
        
        private static SubscriptionReminder MapToSubscriptionReminder(Subscription subscription, string alphabotKey)
        {
            var subscriptionReminder = new SubscriptionReminder()
            {
                UserId = subscription.UserId,
                Username = subscription.ProductSetting!.Username,
                AlphabotKey = alphabotKey,
                ProductRoleId = subscription.Product!.RoleId!.Value,
                IsActive = subscription.IsActive,
                EndDate = subscription.EndDate,
                DaysLeft = subscription.IsActive ? (subscription.EndDate - DateTime.UtcNow).Days : 0
            };

            return subscriptionReminder;
        }

        private async Task SendUsersMetricsAsync(CancellationToken stoppingToken, IEnumerable<ulong> usersWithInactivatedSubs)
        {
            if(!usersWithInactivatedSubs.Any())
            {
                return;
            }
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            IEnumerable<User> allUsers = await userService.GetUsersWithMetricsAsync(stoppingToken);
            List<User> users = allUsers.Where(u => usersWithInactivatedSubs.Contains(u.Id)).ToList();
            Console.WriteLine("-------------------- -------------- ENTREI ReceiveUsersMetrics ------------------ ----------------- ");
            await _hubContext.Clients.All.ReceiveUsersMetrics(users);
        }

       
    }
}