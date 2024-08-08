using ProPayments.Service.Dtos.Subscriptions.Response;

namespace ProPayments.Service.Dtos.Users.Response
{
    public class UserWithSubscriptionsResponse
    {
        public ulong Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string WalletAddress { get; set; }
        public string? Email { get; set; }
        public List<SubscriptionResponse> Subscriptions { get; set; }
    }
}
