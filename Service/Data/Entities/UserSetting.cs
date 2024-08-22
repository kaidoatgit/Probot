using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities;

public abstract class UserSetting
{
    [Column(Order = 0)]
    public ulong Id { get; set; }
    public ulong UserId { get; set; }
    public User User { get; set; } = null!;
    public Subscription Subscription { get; set; } = null!; // Navigation to Subscription
}