using ProPayments.Client.Dtos.Subscription.Response;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Order.Response
{
    public class OrderResult
    {
        public ulong OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public SubscriptionResponse? Subscription { get; set; }
    }

    //public class NewSubscription
    //{
    //    public ulong SubscriptionId { get; set; }
    //    public ulong UserId { get; set; }
    //    public ulong PlanRoleId { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //}
}
