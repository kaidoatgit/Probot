using ProPayments.Service.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class Invoice
    {
        //Invoice details
        [Column(Order = 0)]
        public ulong Id { get; set; }
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //User details
        [Column(Order = 2)]
        public ulong UserId { get; set; }
        [Column(Order = 3)]
        public string? Username { get; set; }

        // Order details
        [Column(Order = 4)]
        public ulong? OrderId { get; set; }
        [Column(Order = 5)]
        public OrderStatus OrderStatus { get; set; }
        [Column(Order = 6)]
        public DateTimeOffset OrderExpiryTime { get; set; }
        public Order Order { get; set; } = null!; // Navigation purpose

        // Transaction details
        [Column(Order = 7)]
        public ulong TransactionId { get; set; }
        [Column(Order = 8)]
        public decimal TotalAmount { get; set; }

        [Column(Order = 9)]
        public Token Token { get; set; }
        [Column(Order = 10)]
        public string? TransactionHash { get; set; }
        [Column(Order = 11)]
        public DateTime? PaymentDate { get; set; }
        [Column(Order = 12)]
        public string? PaymentAddress { get; set; }
        [Column(Order = 13)]
        public string? RecipientAddress { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        internal void UpdateCodes(IEnumerable<ProductKey> productKeys)
        {
            foreach (var productKey in productKeys)
            {
                var invoiceItem = InvoiceItems
                    .FirstOrDefault(ii => ii.Id == productKey.OrderItemId && ii.ProductOptionId == productKey.ProductOptionId);
                if (invoiceItem != null)
                {
                    invoiceItem.ProductKeyCode = productKey.Code;
                }
            }
        }

        internal void UpdateOrderData(OrderStatus orderStatus)
        {
            OrderStatus = orderStatus;
        }
        
        internal void UpdateTransactionData(Transaction transaction)
        {
            TransactionHash = transaction.Hash;
            PaymentDate = transaction.PaymentDate;
        }
    }
}
