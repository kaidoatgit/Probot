namespace ProPayments.Client.Dtos.Subscription.Response
{
    public class SubscriptionResponse
    {
        public ulong UserId { get; set; }
        public ulong PlanRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
