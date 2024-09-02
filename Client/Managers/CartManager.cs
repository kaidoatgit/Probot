using System.Collections.Concurrent;
using Probot.Client.Models;

namespace Probot.Client.Managers;

public class CartManager
{
    private readonly ConcurrentDictionary<ulong, Cart> _shoppingCarts = new();

    public ConcurrentDictionary<ulong, Cart> ShoppingCarts => _shoppingCarts;
    public void InitCart(ulong cartId)
    {
        _shoppingCarts.TryAdd(cartId, new Cart());
    }

    public Cart? GetCart(ulong cartId)
    {
        _shoppingCarts.TryGetValue(cartId, out var cart);
        return cart;
    }
    public void RemoveCart(ulong id)
    {
        _shoppingCarts.TryRemove(id, out _);
    }

    public List<CartItem>? GetItemsFromCart(ulong cartId)
    {
        _shoppingCarts.TryGetValue(cartId, out var cart);
        return cart?.CartItems;
    }

    public void AddItemToCart(ulong cartId, CartItem item)
    {
        _shoppingCarts.TryGetValue(cartId, out var cart);
        cart?.CartItems.Add(item);
    }

    public int GetMaxCartItemId(ulong cartId)
    {
        if (!_shoppingCarts.TryGetValue(cartId, out var cart) || !cart.CartItems.Any())
        {
            return 0; 
        }

        return cart.CartItems.Max(item => item.ItemId);
    }


    public bool RemoveItemFromCart(ulong cartId, int itemId)
    {
        var cart = GetCart(cartId);
        if(cart == null || !cart.CartItems.Any())
        {   
            return false;
        }
        
        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ItemId == itemId);
        if(cartItem == null)
        {
            return false;
        } 

        cart.CartItems.Remove(cartItem);
        return true;
    }

}
