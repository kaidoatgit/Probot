namespace ProPayments.Service.Dtos.Subscriptions.Request
{
    public class SubscriptionRequest
    {
        public ulong UserId { get; set; }
        public int PlanOptionId { get; set; }
    }
}
