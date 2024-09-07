using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
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
    public ICollection<ProductSetting> ProductSettings { get; set; } = new List<ProductSetting>(); //Navigation purpose

    [NotMapped]
    public Metrics? Metrics { get; set; }
}

public class Metrics
{
    public Dictionary<ulong, int> InactiveKeysPerProduct { get; set; } = new();
    public Dictionary<ulong, int> ActiveSubsPerProduct { get; set; } = new();
}