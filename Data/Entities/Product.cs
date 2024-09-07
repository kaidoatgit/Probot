using Probot.Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
public class Product
{
    [Column(Order = 0)]
    public int Id { get; set; }
    [Column(Order = 1)]
    public ulong? RoleId { get; set; }
    [Column(Order = 2)]
    public ProductName Name { get; set; }
    [Column(Order = 3)]
    public string? Description { get; set; }

    public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();  //Navigation purpose
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();  //Navigation purpose
}
