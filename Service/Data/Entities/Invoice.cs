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

        // Invoice items (representing subscriptions in the invoice)
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        // //Invoice details
        // [Column(Order = 0)]
        // public ulong Id { get; set; }
        // [Column(Order = 1)]
        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // //User details
        // [Column(Order = 3)]
        // public ulong UserId { get; set; }
        // [Column(Order = 4)]
        // public string? Username { get; set; }

        // //Order details
        // [Column(Order = 5)]
        // public ulong? OrderId { get; set; }
        // [Column(Order = 6)]
        // public OrderStatus OrderStatus { get; set; }
        // [Column(Order = 7)]
        // public DateTimeOffset OrderExpiryTime { get; set; }
        // public Order? Order { get; set; } //Navigation purpose

        // //Transaction details
        // [Column(Order = 8)]
        // public ulong TransactionId { get; set; }
        // [Column(Order = 9)]
        // public decimal TotalAmount { get; set; }
        // [Column(Order = 10)]
        // public Token Token { get; set; }
        // [Column(Order = 11)]
        // public string? TransactionHash { get; set; }
        // [Column(Order = 12)]
        // public DateTime? PaymentDate { get; set; }
        // [Column(Order = 13)]
        // public string? PaymentAddress { get; set; }
        // [Column(Order = 14)]
        // public string? RecipientAddress { get; set; }

        // //Plan details
        // [Column(Order = 15)]
        // public int PlanId{ get; set; }
        // [Column(Order = 16)]
        // public PlanType PlanType { get; set; }
        // [Column(Order = 17)]
        // public ulong? PlanRoleId { get; set; }
        // [Column(Order = 18)]
        // public int PlanOptionId { get; set; }
        // [Column(Order = 19)]
        // public int PlanPeriod { get; set; }
        // [Column(Order = 20)]
        // public string? PeriodDescription { get; set; }

        // //Subscription details
        // [Column(Order = 21)]
        // public ulong? SubscriptionId { get; set; }
        // [Column(Order = 22)]
        // public DateTime? SubscriptionStartDate { get; set; }
        // [Column(Order = 23)]
        // public DateTime? SubscriptionEndDate { get; set; }

        internal void UpdateOrderData(OrderStatus orderStatus)
        {
            OrderStatus = orderStatus;
        }
        
        internal void UpdateTransactionData(Transaction transaction)
        {
            TransactionHash = transaction.Hash;
            PaymentDate = transaction.PaymentDate;
        }
        // public void UpdateSubscriptionData(Subscription subscription)
        // {
        //     SubscriptionId = subscription.SubscriptionId;
        //     SubscriptionStartDate = subscription.StartDate;
        //     SubscriptionEndDate = subscription.EndDate;
        // }

    }
}
