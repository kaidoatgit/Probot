using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Orders.Response;
using ProPayments.Service.Dtos.Subscriptions.Response;

namespace ProPayments.Service.Services.Hubs.IClients
{
    public interface INotificationClient
    {
        Task ReceiveOrderResult(OrderResult response);
        Task ReceiveSubscriptionsReminders(Dictionary<ulong, List<SubscriptionReminder>> userSubscriptionsReminders);
        Task ReceiveUsers(HashSet<User> userSummary);
    }
}
