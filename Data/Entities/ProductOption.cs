using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
public class ProductOption
{
    [Column(Order = 0)]
    public int Id { get; set; }
    [Column(Order = 1)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column(Order = 2)]
    public decimal Price { get; set; }
    [Column(Order = 3)]
    public int Period { get; set; }
    [Column(Order = 4)]
    public string PeriodDescription { get; set; } = string.Empty;

    [Column(Order = 5)]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!; //Navigation purpose

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); //Navigation purpose
    public ICollection<ProductKey> ProductKeys { get; set; } = new List<ProductKey>(); //Navigation purpose
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();  //Navigation purpose

}

