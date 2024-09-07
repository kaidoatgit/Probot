using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
public abstract class ProductSetting
{
    [Column(Order = 0)]
    public ulong Id { get; set; }
    [Column(Order = 1)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column(Order = 2)]
    public string Username { get; set; } = null!;
    [Column(Order = 3)]
    public ulong UserId { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>(); // Navigation to Subscription
}