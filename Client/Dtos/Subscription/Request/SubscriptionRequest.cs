namespace ProPayments.Client.Dtos.Subscription.Request
{
    public class SubscriptionRequest
    {
        public ulong UserId { get; set; }
        public int PlanOptionId { get; set; }
    }
}
