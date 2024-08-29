namespace ProPayments.Client.Dtos.Subscription.Response
{
    public class UserSummaryResponse
    {
        public ulong Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string WalletAddress { get; set; } = string.Empty;

        public Dictionary<ulong, int> InactiveKeysPerProduct { get; set; } = new();
        public Dictionary<ulong, int> ActiveSubsPerProduct { get; set; } = new();
    }
}
