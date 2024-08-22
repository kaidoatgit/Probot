using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Dtos.Products.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public ProductName Name { get; set; }
        public string? Description { get; set; }
    }
}
