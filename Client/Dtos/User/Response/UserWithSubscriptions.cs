namespace ProPayments.Client.Dtos.Subscription.Response
{
    public class UserWithSubscriptions
    {
        public ulong Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string WalletAddress { get; set; } = string.Empty;
        public List<SubscriptionResponse> Subscriptions { get; set; } = new();
    }
}
