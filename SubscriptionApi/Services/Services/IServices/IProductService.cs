using Probot.Data.Entities;
using Probot.Shared.Dtos.Product.Request;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Product>> GetProductsWithOptionsAsync();
        Task<IEnumerable<ProductOption>> GetProductOptionsAsync();
        Task<bool> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> request);

    }
}
