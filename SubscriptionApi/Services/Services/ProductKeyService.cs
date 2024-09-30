using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.Shared.Enums;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Services.Services.IServices;

namespace Probot.SubscriptionApi.Services.Services;

public class ProductKeyService : IProductKeyService
{
    private readonly ProbotContext _context;
    private readonly IProductService _productService;
    public ProductKeyService(ProbotContext context, IProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<IEnumerable<ProductKey>> GenerateProductKeysAsync(Order order, CancellationToken cancellationToken)
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

    public async Task<IEnumerable<ProductKey>> GenerateProductKeysAsync(int amount, int period)
    {
        var productOptions = await _productService.GetProductOptionsAsync();
        var productOption = productOptions.Where(po => po.Period == period).First();
        List<ProductKey> productKeys = new();
        for(int i=0; i<amount; i++)
        {
            var productKey = new ProductKey
            {
                Period = productOption.Period,
                ProductOptionId = productOption.Id
            };
            productKeys.Add(productKey);
        }
        _context.ProductKeys.AddRange(productKeys);
        await _context.SaveChangesAsync();
        return productKeys;
    }

    public async Task<ProductKey> GetProductKeyByCodeAsync(string code, bool includeReferences = false, ulong? userId = null, bool? isActivated = null)
    {
        IQueryable<ProductKey> query = _context.ProductKeys
            .AsNoTracking()
            .Where(pk => pk.Code == code)
            .Include(pk => pk.ProductOption);

        if(userId.HasValue)
        {
            query = query.Where(pk => pk.UserId == userId);
        }

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
            ?? throw new SubscriptionException(ExceptionResult.ProductKeyNotFound404, $"Product Key with code: {code} not found.");
    }

    public async Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong? userId, bool? isActivated)
    {
        IQueryable<ProductKey> query = _context.ProductKeys
            .AsNoTracking()
            .Include(po => po.ProductOption);

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

    public async Task<ProductKey> ClaimProductKeyAsync(string code, ulong userId)
    {
        var productKey = await GetProductKeyByCodeAsync(code);
        if(productKey.UserId.HasValue)
        {
            throw new SubscriptionException(ExceptionResult.ProductKeyConflict409, $"Product Key with code: {code} already claimed.");
        }
        _context.Attach(productKey);
        productKey.UserId = userId;
        await _context.SaveChangesAsync();
        return productKey;
    }
}
