using ProPayments.Service.Dtos.Orders.Response;
using ProPayments.Service.Dtos.Subscriptions.Response;

namespace ProPayments.Service.Services.Hubs.IClients
{
    public interface INotificationClient
    {
        Task ReceiveOrderResult(OrderResult response);
        Task ReceiveSubscriptionReminder(SubscriptionReminder response);
    }
}
