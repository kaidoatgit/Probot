using Newtonsoft.Json;

namespace ProPayments.Client.Dtos.Subscription.Response
{
    public class SubscriptionReminder
    {
        public ulong UserId { get; set; }
        public string? Username { get; set; }
        public ulong PlanRoleId { get; set; }
        public bool IsToNotifyUser { get; set; }
        public bool IsSubscriptionActive { get; set; }
        public DateTimeOffset SubscriptionEndDate { get; set; }
        public int DaysLeft { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
