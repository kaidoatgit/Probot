using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Data.Entities.Enums;
using ProPayments.Service.Dtos.Subscriptions.Response;
using ProPayments.Service.Services.Hubs;
using ProPayments.Service.Services.Hubs.IClients;

namespace ProPayments.Service.Services.BackgroundServices
{
    public class SubscriptionCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private static readonly TimeSpan _checkingPeriod = TimeSpan.FromHours(1);
        private static readonly List<TimeSpan> _notificationPeriods = new()
        {
            TimeSpan.Zero,
            TimeSpan.FromDays(1),
            TimeSpan.FromDays(3),
            TimeSpan.FromDays(7)
        };

        //private static readonly TimeSpan _checkingPeriod = TimeSpan.FromSeconds(30);
        //private static readonly List<TimeSpan> _notificationPeriods = new()
        //{
        //    TimeSpan.Zero,
        //    TimeSpan.FromSeconds(60),
        //    TimeSpan.FromSeconds(180),
        //    TimeSpan.FromSeconds(300)
        //};

        public SubscriptionCheckService(IServiceProvider serviceProvider, IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_checkingPeriod);
            var updatedSubscriptions = new List<Subscription>();

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
                var now = DateTime.UtcNow;
                var endThreshold = now.AddDays(-3);


                var subscriptions = await dbContext.Subscriptions
                    // .Include(s => s.Product)
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .ToListAsync(stoppingToken);


                foreach (var subscription in subscriptions)
                {
                    TimeSpan timeLeft = subscription.EndDate - now;
                    DateTime lastNotificationSent = subscription.LastNotificationCheck ?? DateTime.MinValue;
                    bool isToNotifyUser = true;

                    // if (subscription.Product!.Type == ProductName.Free)
                    // {
                    //     if (timeLeft <= TimeSpan.Zero)
                    //     {
                    //         subscription.IsActive = false;
                    //         subscription.LastNotificationCheck = subscription.EndDate;
                    //         updatedSubscriptions.Add(subscription);

                    //         isToNotifyUser = false;
                    //         var notificationResponse = MapToSubscriptionReminder(subscription, isToNotifyUser);
                    //         await _hubContext.Clients.All.ReceiveSubscriptionReminder(notificationResponse);
                    //         continue;
                    //     }
                    //     else
                    //     {
                    //         continue;
                    //     }
                    // }

                    foreach (var period in _notificationPeriods)
                    {
                        var notificationTime = subscription.EndDate - period;

                        if (timeLeft <= period && notificationTime > lastNotificationSent)
                        {
                            if (period == TimeSpan.Zero)
                            {
                                if (endThreshold > subscription.EndDate)
                                {
                                    isToNotifyUser = false;
                                }
                                subscription.IsActive = false;
                            }
                            subscription.LastNotificationCheck = notificationTime;
                            updatedSubscriptions.Add(subscription);

                            var subscriptionReminder = MapToSubscriptionReminder(subscription, isToNotifyUser);
                            await _hubContext.Clients.All.ReceiveSubscriptionReminder(subscriptionReminder);
                            break;
                        }
                    }
                }

                // Batch update subscriptions
                if (updatedSubscriptions.Any())
                {
                    dbContext.Subscriptions.UpdateRange(updatedSubscriptions);
                    updatedSubscriptions.Clear();
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
        }

        private static SubscriptionReminder MapToSubscriptionReminder(Subscription subscription, bool isToNotifyUser)
        {
            var subscriptionReminder = new SubscriptionReminder()
            {
                UserId = subscription.UserId,
                Username = subscription.Username,
                // ProductRoleId = subscription.Product!.RoleId!.Value,
                IsToNotifyUser = isToNotifyUser,
                IsSubscriptionActive = subscription.IsActive,
                SubscriptionEndDate = subscription.EndDate,
                DaysLeft = subscription.IsActive ? (DateTime.UtcNow - subscription.EndDate).Days : 0
            };

            return subscriptionReminder;
        }
    }
}