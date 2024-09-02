namespace Probot.Client.Models;

public class Cart
{
    public static readonly int MaxItemsPerCart = 15;
    public List<CartItem> CartItems { get; set; } = new();
}
