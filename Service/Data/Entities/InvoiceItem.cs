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

        // product details
        [Column(Order = 2)]
        public int ProductId { get; set; }
        [Column(Order = 3)]
        public ProductName ProductName { get; set; }
        [Column(Order = 4)]
        public ulong? ProductRoleId { get; set; }
        [Column(Order = 5)]
        public int ProductOptionId { get; set; }
        [Column(Order = 6)]
        public decimal ProductOptionPrice { get; set; }
        [Column(Order = 7)]
        public int ProductOptionPeriod { get; set; }
        [Column(Order = 8)]
        public string? PeriodDescription { get; set; }

        // product key details
        [Column(Order = 9)]
        public string? ProductKeyCode { get; set; }
    }
} 