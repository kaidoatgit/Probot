using System.Text;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Models
{
    public class Invoice
    {
        public ulong Id { get; set; }
        public string PaymentAddress { get; set; } = string.Empty;
        public DateTimeOffset OrderExpiryTime { get; set; }
        public decimal TotalAmount { get; set; }
        public Token Token { get; set; }
        public string RecipientAddress { get; set; } = string.Empty;
        public List<InvoiceItem> InvoiceItems { get; set; } = new();
    }

    public class InvoiceItem
    {
        public ProductName ProductName { get; set; }
        public ulong ProductRoleId { get; set; }
        public decimal ProductOptionPrice { get; set; }
        public int ProductOptionPeriod { get; set; }
        public string PeriodDescription { get; set; } = string.Empty;

        public override string ToString()
        {
            StringBuilder invoiceItem = new();
            invoiceItem.Append($"{ProductName.ToString().PadRight(22)}| ");
            invoiceItem.Append($"{PeriodDescription.PadRight(14)}| ");
            invoiceItem.Append($"${ProductOptionPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).PadRight(8)}");
            return invoiceItem.ToString();
        }
    }
}
