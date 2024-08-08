namespace ProPayments.Service.Dtos.Subscriptions.Response
{
    public class SubscriptionResponse
    {
        public ulong SubscriptionId { get; set; }
        public ulong UserId { get; set; }
        public ulong PlanRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
