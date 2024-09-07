using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
public class Subscription
{
    [Column(Order = 0)]
    public ulong Id { get; set; }
    [Column(Order = 1)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(Order = 2)]
    public string Code { get; set; } = null!;
    
    [Column(Order = 3)]
    public ulong UserId { get; set; }
    [Column(Order = 4)]
    public string? Username { get; set; }

    [Column(Order = 5)]
    public int ProductId { get; set; }
    [Column(Order = 6)]
    public ulong? ProductSettingId { get; set; } 

    [Column(Order = 7)]
    public int Version { get; set; } = 1;
    [Column(Order = 8)]
    public bool IsActive { get; set; } = true;
    [Column(Order = 9)]
    public DateTime StartDate { get; set; }
    [Column(Order = 10)]
    public DateTime EndDate { get; set; }
    [Column(Order = 11)]
    public DateTime? LastNotificationCheck { get; set; }

    public ProductSetting? ProductSetting { get; set; } // Navigation purpose
    public Product? Product { get; set; } //Navigation purpose
    public User User { get; set; } = null!; //Navigation purpose
    [ForeignKey("Code")]
    public ProductKey? ProductKey { get; set; } //Navigation purpose
}