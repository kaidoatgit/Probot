using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class User
    {
        [Column(Order = 0)]
        public ulong Id { get; set; }
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column(Order = 2)]
        [StringLength(100, ErrorMessage = "Username length can't be more than 100.")]
        public string Username { get; set; } = string.Empty;
        [Column(Order = 3)]
        public string WalletAddress { get; set; } = string.Empty;
        [Column(Order = 4)]
        public string? Email { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>(); //Navigation purpose
        public ICollection<ProductKey> ProductKeys { get; set; } = new List<ProductKey>(); //Navigation purpose
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>(); //Navigation purpose
        public ICollection<UserSetting> UserSettings { get; set; } = null!;//Navigation purpose
    }
}
