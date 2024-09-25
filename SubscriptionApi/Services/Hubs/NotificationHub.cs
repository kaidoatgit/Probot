using Microsoft.AspNetCore.SignalR;
using Probot.Data.Entities;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Dtos.Subscription.Response;

namespace Probot.SubscriptionApi.Services.Hubs
{
    public interface INotificationClient
    {
        Task ReceiveOrderResult(OrderResult response);
        Task ReceiveSubscriptionsReminders(Dictionary<ulong, List<SubscriptionReminder>> userSubscriptionsReminders);
        Task ReceiveUsersMetrics(List<User> users);
    }

    public class NotificationHub : Hub<INotificationClient> { }
}
