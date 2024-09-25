namespace Probot.Client.Models
{
    public class Transaction
    {
        public string Hash { get; private set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentAddress { get; set; } = string.Empty;
        public DateTime PaymentDate { get; private set; }
    }
}
