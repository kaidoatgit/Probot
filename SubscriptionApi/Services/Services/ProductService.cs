using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.Product.Request;
using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly ProbotContext _context;
        private readonly Mapper _mapper;

        public ProductService(ProbotContext context, Mapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new SubscriptionException(ExceptionResult.ProductNotFound404, $"Product {productId} not found");
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithOptionsAsync()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductOptions)
                .ToListAsync();
            return products;
        }
      
        public async Task<bool> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> request)
        {
            bool isModified = false;

            var products = request.Select(p => _mapper.MapToProductEntity(p));
            var dbProducts = await _context.Products.ToListAsync();

            foreach (var dbProduct in dbProducts)
            {
                // Find the corresponding update request for the current product type
                var productRequest = products.FirstOrDefault(p => p.Id == dbProduct.Id);

                // If there's a matching update request
                if (productRequest != null)
                {
                    // Update DiscordRoleId if it's null or different
                    if (dbProduct.RoleId == null || dbProduct.RoleId != productRequest.RoleId)
                    {
                        dbProduct.RoleId = productRequest.RoleId;
                        isModified = true;
                    }
                }
            }

            if (isModified)
            {
                await _context.SaveChangesAsync();
            }
            return isModified;
        }
    
        public async Task<IEnumerable<ProductOption>> GetProductOptionsAsync()
        {
            return await _context.ProductOptions.AsNoTracking().ToListAsync();
        }

    }
}
