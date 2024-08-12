using System.ComponentModel.DataAnnotations.Schema;
using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Data.Entities
{
    public class InvoiceItem
    {
        [Column(Order = 0)]
        public ulong Id { get; set; }

        [Column(Order = 1)]
        public ulong InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!; // Navigation purpose

        // plan details
        [Column(Order = 2)]
        public int PlanId { get; set; }
        [Column(Order = 3)]
        public PlanType PlanType { get; set; }
        [Column(Order = 4)]
        public ulong? PlanRoleId { get; set; }
        [Column(Order = 5)]
        public int PlanOptionId { get; set; }
        [Column(Order = 6)]
        public decimal PlanOptionPrice { get; set; }
        [Column(Order = 7)]
        public int PlanOptionPeriod { get; set; }
        [Column(Order = 8)]
        public string? PeriodDescription { get; set; }

        // subscription details
        [Column(Order = 9)]
        public ulong? SubscriptionId { get; set; }
        [Column(Order = 10)]
        public DateTime? SubscriptionStartDate { get; set; }
        [Column(Order = 11)]
        public DateTime? SubscriptionEndDate { get; set; }
        public Subscription Subscription { get; set; } = null!; // Navigation purpose
    }
} 