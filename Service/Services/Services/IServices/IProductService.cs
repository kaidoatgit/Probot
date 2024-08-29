using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Products.Request;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductOption>> GetProductOptionsAsync();
        Task<IEnumerable<Product>> GetProductsWithOptionsAsync();
        Task<bool> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> request);

    }
}
