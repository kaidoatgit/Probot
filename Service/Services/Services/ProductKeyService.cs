using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services;

public class ProductKeyService : IProductKeyService
{
    private readonly SubscriptionContext _context;
    public ProductKeyService(SubscriptionContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductKey>> GenerateProductKeys(Order order)
    {
        List<ProductKey> productKeys = new();
        List<OrderItem> orderItems = order.OrderItems.ToList();
        foreach (var orderItem in orderItems)
        {
            var productKey = new ProductKey
            {
                Period = orderItem.ProductOption.Period,
                ProductOptionId = orderItem.ProductOptionId,
                OrderItemId = orderItem.Id,
                UserId = order.UserId
            };
            productKeys.Add(productKey);
            _context.ProductKeys.Add(productKey);
        }
        await _context.SaveChangesAsync();
        return productKeys;
    }

    public async Task<ProductKey> GetProductKeyAsync(string code, ulong userId, bool isActivated, bool includeReferences=false)
    {
        IQueryable<ProductKey> query = _context.ProductKeys.AsTracking()
            .Where(pk => pk.Code == code && pk.UserId == userId && pk.IsActivated == isActivated);

        if (includeReferences)
        {
            query = query
                .Include(pk => pk.User)
                .Include(pk => pk.ProductOption.Product);
        }

        var productKey = await query.FirstOrDefaultAsync()
            ?? throw new ServiceException(StatusCodes.Status400BadRequest, "Product Key not found or already activated.");
        return productKey;
    }

    public async Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong userId, bool isActivated)
    {
        var productKeys = await _context.ProductKeys
            .Where(pk => pk.UserId == userId && pk.IsActivated == isActivated)
            .ToListAsync();
        return productKeys;
    }
}
