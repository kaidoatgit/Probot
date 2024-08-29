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

    public async Task<IEnumerable<ProductKey>> GenerateProductKeys(Order order, CancellationToken cancellationToken)
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
        
        await _context.SaveChangesAsync(cancellationToken);
        return productKeys;
    }

    public async Task<ProductKey> GetProductKeyByCodeAsync(string code, bool? isActivated, bool includeReferences=false)
    {
        IQueryable<ProductKey> query = _context.ProductKeys
            .AsNoTracking();

        if(!isActivated.HasValue)
        {
            query = query.Where(pk => pk.Code == code);
        }
        else
        {
            query = query.Where(pk => pk.Code == code && pk.IsActivated == isActivated);
        }

        if (includeReferences)
        {
            query = query
                .AsTracking()
                .Include(pk => pk.User)
                .Include(pk => pk.ProductOption.Product);
        }

        return await query.SingleOrDefaultAsync()
            ?? throw new ServiceException(StatusCodes.Status400BadRequest, "Product Key not found or already activated.");
    }

    public async Task<IEnumerable<ProductKey>> GetProductKeysForUserAsync(ulong userId, bool isActivated)
    {
        var productKeys = await _context.ProductKeys
            .AsNoTracking()
            .Where(pk => pk.UserId == userId && pk.IsActivated == isActivated)
            .ToListAsync();
        return productKeys;
    }

    public async Task<Dictionary<ulong, int>> GetNonActivatedProductKeysPerUserAsync(CancellationToken cancellationToken)
    {
        var nonActivatedKeys = await _context.ProductKeys
            .AsNoTracking()
            .Where(pk => !pk.IsActivated)
                .Include(s => s.ProductOption)
                    .ThenInclude(po => po!.Product)
            .GroupBy(pk => pk.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                NonActivatedKeyCount = g.Count()
            })
            .ToDictionaryAsync(g => g.UserId, g => g.NonActivatedKeyCount, cancellationToken);

        return nonActivatedKeys;
    }
}
