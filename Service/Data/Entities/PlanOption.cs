using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class PlanOption
    {
        [Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column(Order = 2)]
        public decimal Price { get; set; }
        [Column(Order = 3)]
        public int Period { get; set; }
        [Column(Order = 4)]
        public string PeriodDescription { get; set; } = string.Empty;

        [Column(Order = 5)]
        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!; //Navigation purpose

        public ICollection<OrderItem>? OrderItems { get; set; }  //Navigation purpose
        public ICollection<Subscription>? Subscriptions { get; set; }  //Navigation purpose

    }
}
