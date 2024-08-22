using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Product.Response
{
    public class ProductWithOptionsResponse
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public ProductName Name { get; set; }
        public string? Description { get; set; }
        public List<ProductOptionResponse> ProductOptions { get; set; } = new();
    }

    public class ProductOptionResponse
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Period { get; set; }
        public string PeriodDescription { get; set; } = string.Empty;
    }
}
