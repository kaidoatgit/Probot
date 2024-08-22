using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class Subscription
    {
        [Column(Order = 0)]
        public ulong Id { get; set; }
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column(Order = 2)]
        public DateTime? UpdatedAt { get; set; }
        [Column(Order = 3)]
        public DateTime StartDate { get; set; }
        [Column(Order = 4)]
        public DateTime EndDate { get; set; }
        [Column(Order = 5)]
        public bool IsActive { get; set; } = true;
        [Column(Order = 6)]
        public DateTime? LastNotificationCheck { get; set; }


        [Column(Order = 7)]
        public ulong UserId { get; set; }
        [Column(Order = 8)]
        public string? Username { get; set; }
        public User User { get; set; } = null!; //Navigation purpose

        [Column(Order = 9)]
        public int ProductOptionId { get; set; }
        public ProductOption ProductOption { get; set; } = null!; //Navigation purpose

        [Column(Order = 10)]
        public string Code { get; set; } = null!;
        [ForeignKey("Code")]
        public ProductKey ProductKey { get; set; } = null!; //Navigation purpose
        
        [Column(Order = 11)]
        public ulong UserSettingId { get; set; } 
        public UserSetting UserSetting { get; set; } = null!;// Navigation purpose
    }
}
