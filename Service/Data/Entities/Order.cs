using ProPayments.Service.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class Order
    {
        //Order details
        [Column(Order = 0)]
        public ulong Id { get; set; }
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column(Order = 2)]
        public DateTimeOffset ExpiryTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(5);
        [Column(Order = 3)]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        //User details
        [Column(Order = 4)]
        public ulong UserId { get; set; }
        public User User { get; set; } = null!; //Navigation purpose

        //Tansaction details
        [Column(Order = 5)]
        public ulong TransactionId { get; set; }
        public Transaction? Transaction { get; set; } // Navigation purpose


        //Invoice details
        [Column(Order = 6)]
        public ulong InvoiceId { get; set; }
        public Invoice? Invoice { get; set; } // Navigation purpose

        // OrderItems details
        public ICollection<OrderItem> OrderItems { get; set; } = null!; // Navigation purpose


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
