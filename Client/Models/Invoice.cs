using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Models
{
    public class Invoice
    {
        public ulong Id { get; set; }
        public string PaymentAddress { get; set; } = string.Empty;
        public DateTimeOffset OrderExpiryTime { get; set; }
        public ulong PlanRoleId { get; set; }
        public int PlanPeriod { get; set; }
        public string PeriodDescription { get; set; }
        public decimal TotalAmount { get; set; }
        public string RecipientAddress { get; set; }
        public Token Token { get; set; }
    }
}
