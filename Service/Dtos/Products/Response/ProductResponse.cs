using ProPayments.Service.Data.Entities.Enums;
using ProPayments.Service.Dtos.ProductOptions.Response;

namespace ProPayments.Service.Dtos.Products.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public ProductName Name { get; set; }
        public string? Description { get; set; }
        
        public List<ProductOptionResponse>? ProductOptions { get; set; }
    }
}
