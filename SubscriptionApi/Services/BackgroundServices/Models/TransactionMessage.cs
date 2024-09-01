namespace Probot.SubscriptionApi.Services.BackgroundServices.Models
{
    public class TransactionMessage
    {
        public string Hash { get; }
        public decimal Amount { get; }
        public string Address { get; }

        public TransactionMessage(string hash, string address, decimal amount)
        {
            Hash = hash;
            Amount = amount;
            Address = address;
        }
    }
}
