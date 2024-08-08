namespace ProPayments.Client.Dtos.User.Request
{
    public class UserRequest
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string WalletAddress { get; set; }
    }
}
