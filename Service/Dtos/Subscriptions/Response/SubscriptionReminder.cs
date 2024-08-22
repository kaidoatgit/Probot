namespace ProPayments.Service.Dtos.Subscriptions.Response
{
    public class SubscriptionReminder
    {
        public ulong UserId { get; set; }
        public string? Username { get; set; }
        public ulong ProductRoleId { get; set; }
        public bool IsToNotifyUser { get; set; }
        public bool IsSubscriptionActive { get; set; }
        public DateTimeOffset SubscriptionEndDate { get; set; }
        public int DaysLeft { get; set; }
    }
}
