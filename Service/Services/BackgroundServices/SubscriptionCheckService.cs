using System.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Subscriptions.Response;
using ProPayments.Service.Dtos.Users.Response;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Hubs;
using ProPayments.Service.Services.Hubs.IClients;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.BackgroundServices
{
    public partial class SubscriptionCheckService : BackgroundService
    {
        // private static readonly TimeSpan _reminderPeriod = TimeSpan.FromHours(1);
        // private static readonly List<TimeSpan> _notificationPeriods = new()
        // {
        //     TimeSpan.Zero,
        //     TimeSpan.FromDays(1),
        //     TimeSpan.FromDays(3),
        //     TimeSpan.FromDays(7)
        // };

        private static readonly TimeSpan _reminderPeriod = TimeSpan.FromSeconds(1);
        private static readonly List<TimeSpan> _notificationPeriods = new()
        {
           TimeSpan.Zero,
        //    TimeSpan.FromMinutes(1),
        //    TimeSpan.FromMinutes(2),
           TimeSpan.FromMinutes(3)
        };
        
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly Mapper _mapper;

        public SubscriptionCheckService(IServiceProvider serviceProvider, IHubContext<NotificationHub, INotificationClient> hubContext, Mapper mapper)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        #region Sample concurrency for testing
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_reminderPeriod);
            var subsReminders = new List<SubscriptionReminder>();

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                var reminderTask = SendSubscriptionsReminderPerUser(stoppingToken)
                                    .ContinueWith(t => SendUsersData(stoppingToken));
                await reminderTask;
            }
            #region different approach
            // if(subscriptionsToUpdate.Count > 0)
            // {
            //     var saved = false;
            //     while(!saved)
            //     {
            //         try
            //         {
            //             await dbContext.SaveChangesAsync(stoppingToken);
            //             saved = true;
            //         }
            //         catch (DbUpdateConcurrencyException ex)
            //         {
            //             Console.WriteLine($"Concurrency exception: {ex.Message}");
            //             foreach (var entry in ex.Entries)
            //             {
            //                 var proRaffle = (ProRaffle) entry.Entity;
            //                 dbContext.Entry(proRaffle.Subscriptions.First()).State = EntityState.Detached;
            //                 dbContext.Entry(proRaffle).State = EntityState.Detached;
            //             }
            //         }
            //     }
            // }
            #endregion
        }
        #endregion

        private async Task SendSubscriptionsReminderPerUser(CancellationToken stoppingToken)
        {
            var subsReminders = new List<SubscriptionReminder>();
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
            var subscriptions = await dbContext.Subscriptions
                .Where(s => s.Id == 2 || s.Id == 5 || s.Id == 8 || s.Id == 12)
                .Include(s => s.UserSetting)
                .Include(s => s.ProductOption)
                    .ThenInclude(po => po.Product)
                .ToListAsync(cancellationToken: stoppingToken);

            foreach(var subscription in subscriptions)
            {
                if (subscription.UserSetting is ProRaffle proRaffle)
                {
                    proRaffle.IsPaused = true;
                    subscription.IsActive = false;

                    try
                    {
                        await dbContext.SaveChangesAsync(stoppingToken);
                        subsReminders.Add(MapToSubscriptionReminder(subscription, proRaffle.Key));
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Console.WriteLine($"Concurrency exception: {ex.Message}");
                        dbContext.Entry(subscription).State = EntityState.Detached;
                        dbContext.Entry(proRaffle).State = EntityState.Detached;
                    }   
                }
            }
            
            if (subsReminders.Any())
            {
                Console.WriteLine("-------------------- -------------- ENTREI SubscriptionsReminder ------------------ ----------------- ");
                Dictionary<ulong, List<SubscriptionReminder>> userSubscriptionsReminders = subsReminders
                    .GroupBy(s => s.UserId)
                    .ToDictionary(g => g.Key, g => g.ToList());
                await _hubContext.Clients.All.ReceiveSubscriptionsReminders(userSubscriptionsReminders);
            }
        }

        private async Task SendUsersData(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var subscriptionServiceService = scope.ServiceProvider.GetRequiredService<IUserService>();
            HashSet<User> users = await subscriptionServiceService.GetUsersAsync(stoppingToken);
            // List<UserSummaryResponse> usersWithSummary = users.Select(u => _mapper.MapToUserSummaryResponse(u)).ToList();
            Console.WriteLine("-------------------- -------------- ENTREI SendUsersSummary ------------------ ----------------- ");
            await _hubContext.Clients.All.ReceiveUsers(users);
        }







        // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        // {
        //     using var timer = new PeriodicTimer(_checkingPeriod);
        //     var subsReminder = new List<SubscriptionReminder>();

        //     while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        //     {
        //         using var scope = _serviceProvider.CreateScope();
        //         var dbContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
        //         var now = DateTime.UtcNow;

        //         var subscriptions = await dbContext.Subscriptions
        //             .Where(s => s.IsActive)
        //             .Include(s => s.UserSetting)
        //             .Include(s => s.ProductOption)
        //                 .ThenInclude(po => po.Product)
        //             .ToListAsync(stoppingToken);

        //         foreach (var subscription in subscriptions)
        //         {
        //             TimeSpan timeLeft = subscription.EndDate - now;
        //             DateTime lastNotificationSent = subscription.LastNotificationCheck ?? DateTime.MinValue;
        //             string alphabotKey = string.Empty;

        //             foreach (var period in _notificationPeriods)
        //             {
        //                 var notificationTime = subscription.EndDate - period;
        //                 if (timeLeft <= period && notificationTime > lastNotificationSent)
        //                 {     
        //                     if (subscription.UserSetting is ProRaffle proRaffle)
        //                     {                   
        //                         if (period == TimeSpan.Zero)
        //                         {
        //                             subscription.IsActive = false;
        //                             proRaffle.IsPaused = true;
        //                         }
        //                         subscription.LastNotificationCheck = notificationTime;
        //                         try
        //                         {
        //                             await dbContext.SaveChangesAsync(stoppingToken);
        //                             subsReminder.Add(MapToSubscriptionReminder(subscription, proRaffle.Key));
        //                         }
        //                         catch (DbUpdateConcurrencyException ex)
        //                         {
        //                             Console.WriteLine($"Concurrency exception: {ex.Message}");
        //                             dbContext.Entry(subscription).State = EntityState.Detached;
        //                             dbContext.Entry(proRaffle).State = EntityState.Detached;
        //                         }
        //                     }                    
        //                     break;
        //                 }
        //             }
        //         }

        //         if (subsReminder.Any())
        //         {
        //             var groupedSubscriptions = subsReminder
        //                 .GroupBy(s => s.UserId)
        //                 .ToList();
        //             await _hubContext.Clients.All.ReceiveSubscriptionsReminder(groupedSubscriptions);
        //             subsReminder.Clear();
        //         }
        //     }
        // }

        private static SubscriptionReminder MapToSubscriptionReminder(Subscription subscription, string alphabotKey)
        {
            var subscriptionReminder = new SubscriptionReminder()
            {
                UserId = subscription.UserId,
                Username = subscription.Username,
                AlphabotKey = alphabotKey,
                ProductRoleId = subscription.ProductOption!.Product.RoleId!.Value,
                IsActive = subscription.EndDate > DateTime.UtcNow,//subsubscription.IsActive,
                EndDate = subscription.EndDate,
                DaysLeft = subscription.IsActive ? (DateTime.UtcNow - subscription.EndDate).Days : 0
            };

            return subscriptionReminder;
        }
    }
}