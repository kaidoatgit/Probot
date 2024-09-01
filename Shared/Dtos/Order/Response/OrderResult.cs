using Probot.Shared.Enums;

namespace Probot.Shared.Dtos.Order.Response;
public class OrderResult
{
    public ulong OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ulong UserId { get; set; }
    public Dictionary<ulong, int> PurchasedKeysCountPerProduct { get; set; } = new();
}
