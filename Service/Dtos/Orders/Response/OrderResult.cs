using ProPayments.Service.Data.Entities.Enums;
using ProPayments.Service.Dtos.Subscriptions.Response;

namespace ProPayments.Service.Dtos.Orders.Response
{
    public class OrderResult
    {
        public ulong OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public SubscriptionResponse? Subscription { get; set; }
    }
}
