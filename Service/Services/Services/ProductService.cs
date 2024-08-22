using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Products.Request;
using ProPayments.Service.Dtos.Products.Response;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly SubscriptionContext _context;
        private readonly Mapper _mapper;

        public ProductService(SubscriptionContext context, Mapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new ServiceException(StatusCodes.Status404NotFound, $"Product {productId} not found");
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<ProductOption>> GetProductOptionsAsync()
        {
            var productOptions = await _context.ProductOptions.ToListAsync();
            return productOptions;
        }

        public async Task<IEnumerable<ProductWithOptionsResponse>> GetProductsWithOptionsAsync()
        {
            var products = await _context.ProductOptions
                .Include(p => p.Product)
                .ToListAsync();

            var optionsGroupedByProduct = products
             .GroupBy(p => p.Product) // Group by Product
             .Select(g => new ProductWithOptionsResponse
             {
                 Id = g.Key!.Id,
                 RoleId = g.Key.RoleId,
                 Name = g.Key.Name,
                 Description = g.Key.Description,
                 ProductOptions = g.Select(p => new ProductOptionResponse
                 {
                     Id = p.Id,
                     Period = p.Period,
                     Price = p.Price,
                     PeriodDescription = p.PeriodDescription
                 }).ToList()
             })
             .ToList(); // Perform grouping and projection in memory

            return optionsGroupedByProduct;
        }

        public async Task<bool> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> request)
        {
            bool isModified = false;

            var products = request.Select(p => _mapper.MapToProductEntity(p));
            var dbProducts = await _context.Products.ToListAsync();

            foreach (var dbProduct in dbProducts)
            {
                // Find the corresponding update request for the current product type
                var productRequest = products.FirstOrDefault(p => p.Name == dbProduct.Name);

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
    }
}
