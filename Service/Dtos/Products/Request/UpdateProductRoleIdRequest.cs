using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Dtos.Products.Request
{
    public class UpdateProductRoleIdRequest
    {
        public ulong? RoleId { get; set; }
        public ProductName Name { get; set; }
    }
}
