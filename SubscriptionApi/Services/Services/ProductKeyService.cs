using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Services.Services.IServices;

namespace Probot.SubscriptionApi.Services.Services;

public class ProductKeyService : IProductKeyService
{
    private readonly ProbotContext _context;
    public ProductKeyService(ProbotContext context)
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

            // Explicitly load the ProductOption
            await _context.Entry(productKey)
                        .Reference(pk => pk.ProductOption)
                        .LoadAsync(cancellationToken);

            // Explicitly load the Product from the ProductOption
            await _context.Entry(productKey.ProductOption)
                        .Reference(po => po.Product)
                        .LoadAsync(cancellationToken);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        return productKeys;
    }

    public async Task<ProductKey> GetProductKeyByCodeAsync(string code, bool? isActivated, bool includeReferences=false)
    {
        IQueryable<ProductKey> query = _context.ProductKeys
            .AsNoTracking()
            .Where(pk => pk.Code == code);

        if(isActivated.HasValue)
        {
            query = query.Where(pk => pk.IsActivated == isActivated);
        }

        if (includeReferences)
        {
            query = query
                .AsTracking()
                .Include(pk => pk.User)
                .Include(pk => pk.ProductOption)
                    .ThenInclude(po => po.Product);
        }

        return await query.SingleOrDefaultAsync()
            ?? throw new ServiceException(StatusCodes.Status404NotFound, $"Product Key with code: {code} not found.");
    }

    public async Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong? userId, bool? isActivated)
    {
        IQueryable<ProductKey> query = _context.ProductKeys
            .AsNoTracking();

        if(userId.HasValue)
        {
            query = query.Where(pk => pk.UserId == userId);
        }
        if(isActivated.HasValue)
        {
            query = query.Where(pk => pk.IsActivated == isActivated);
        }

        return await query.ToListAsync();
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
