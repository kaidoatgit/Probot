using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
public class OrderItem
{
    [Column(Order = 0)]
    public ulong Id { get; set; }

    [Column(Order = 1)]
    public ulong OrderId { get; set; }
    public Order Order { get; set; } = null!; // Navigation purpose

    [Column(Order = 2)]
    public int ProductOptionId { get; set; }
    public ProductOption ProductOption { get; set; } = null!; // Navigation purpose
    public ProductKey? ProductKey { get; set; } // Navigation purpose
}