using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class Subscription
    {
        [Column(Order = 0)]
        public ulong SubscriptionId { get; set; }
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column(Order = 2)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Column(Order = 3)]
        public DateTime StartDate { get; set; }
        [Column(Order = 4)]
        public DateTime EndDate { get; set; }
        [Column(Order = 5)]
        public bool IsActive { get; set; } = true;
        [Column(Order = 6)]
        public DateTime LastNotificationCheck { get; set; } = DateTime.UtcNow;


        [Column(Order = 7)]
        public ulong UserId { get; set; }
        [Column(Order = 8)]
        public string? Username { get; set; }
        public User? User { get; set; } //Navigation purpose


        [Column(Order = 9)]
        public int PlanId { get; set; }
        public Plan? Plan { get; set; } //Navigation purpose


        [Column(Order = 10)]
        public int PlanOptionId { get; set; }
        [Column(Order = 11)]
        public PlanOption? PlanOption { get; set; }  //Navigation purpose
    }
}
