using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class OrderItem
    {
        [Column(Order = 0)]
        public ulong Id { get; set; }

        [Column(Order = 1)]
        public ulong OrderId { get; set; }
        public Order Order { get; set; } = null!; // Navigation purpose

        [Column(Order = 2)]
        public int PlanOptionId { get; set; }
        public PlanOption PlanOption { get; set; } = null!; // Navigation purpose
    }
}