using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProPayments.Service.Data.Entities;

public class ProductKey
{
    [Key]
    [Column(Order = 0)]
    public string Code { get; set; } = Guid.NewGuid().ToString();
    [Column(Order = 1)]
    public int Period { get; set; }
    [Column(Order = 3)]
    public bool IsActivated { get; set; } = false;

    [Column(Order = 4)]
    public int ProductOptionId { get; set; }
    public ProductOption ProductOption { get; set; } = null!; // Navigation purpose

    [Column(Order = 5)]
    public ulong UserId { get; set; }
    public User User { get; set; } = null!; // Navigation purpose

    [Column(Order = 6)]
    public ulong OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!; // Navigation purpose

    [Column(Order = 7)]
    [ConcurrencyCheck]
    public Guid Version { get; set; }
    public Subscription? Subscription { get; set; }
}

