using ProPayments.Service.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class Order
    {
        [Column(Order = 0)]
        public ulong Id { get; set; }
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column(Order = 2)]
        public DateTimeOffset ExpiryTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(2);
        [Column(Order = 3)]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(Order = 4)]
        public ulong UserId { get; set; }
        public User User { get; set; } = null!; //Navigation purpose

        public Transaction Transaction { get; set; } = null!; // Navigation purpose
        public Invoice Invoice { get; set; } = null!; // Navigation purpose
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Navigation purpose


        internal void Complete()
        {
            Status = OrderStatus.Completed;
        }

        internal bool IsOrderExpirable()
        {
            return Status == OrderStatus.Pending && DateTime.UtcNow > ExpiryTime;
        }
    }

}
