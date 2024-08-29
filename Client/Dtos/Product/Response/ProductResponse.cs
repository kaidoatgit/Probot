using ProPayments.Client.Dtos.ProductOption.Response;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Product.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public ProductName Name { get; set; }
        public string? Description { get; set; }
        public List<ProductOptionResponse> ProductOptions { get; set; } = new();
    }
}
