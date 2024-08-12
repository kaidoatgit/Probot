using System.Collections.Concurrent;
using Microsoft.VisualBasic;
using ProPayments.Client.Models;

namespace ProPayments.Client.Services.Managers;

public class CartManager
{
    private readonly ConcurrentDictionary<ulong, Cart> _shoppingCarts = new();

    public void InitCart(ulong messageId)
    {
        _shoppingCarts.TryAdd(messageId, new Cart());
    }

    public List<CartItem>? GetItemsFromCart(ulong messageId)
    {
        _shoppingCarts.TryGetValue(messageId, out var cart);
        return cart?.CartItems;
    }

    public void AddItemToCart(ulong messageId, CartItem item)
    {
        _shoppingCarts.TryGetValue(messageId, out var cart);
        cart?.CartItems.Add(item);
    }

    public int GetMaxCartItemId(ulong messageId)
    {
        if (!_shoppingCarts.TryGetValue(messageId, out var cart) || !cart.CartItems.Any())
        {
            return 0; 
        }

        return cart.CartItems.Max(item => item.ItemId);
    }

    public Cart? GetCart(ulong messageId)
    {
        _shoppingCarts.TryGetValue(messageId, out var cart);
        return cart;
    }

    public void RemoveItemFromCart(ulong messageId, int productId)
    {
        var cart = GetCart(messageId);
        if(cart != null && cart.CartItems.Any()){
            var cartItem = cart.CartItems.First(ci => ci.ItemId == productId);
            cart.CartItems.Remove(cartItem);
        }
    }
}
