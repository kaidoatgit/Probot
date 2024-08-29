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
        public ulong UserId { get; set; }
        [Column(Order = 3)]
        public string? Username { get; set; }
        public User User { get; set; } = null!; //Navigation purpose
        
        [Column(Order = 4)]
        public ulong UserSettingId { get; set; } 
        public UserSetting? UserSetting { get; set; } // Navigation purpose

        [Column(Order = 5)]
        public int Version { get; set; }
        [Column(Order = 6)]
        public DateTime StartDate { get; set; }
        [Column(Order = 7)]
        public DateTime EndDate { get; set; }
        [Column(Order = 8)]
        public bool IsActive { get; set; } = true;
        [Column(Order = 9)]
        public DateTime? LastNotificationCheck { get; set; }

        [Column(Order = 10)]
        public int ProductOptionId { get; set; }
        public ProductOption? ProductOption { get; set; } //Navigation purpose

        [Column(Order = 11)]
        public string Code { get; set; } = null!;
        [ForeignKey("Code")]
        public ProductKey ProductKey { get; set; } = null!; //Navigation purpose
    }
}
