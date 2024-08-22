using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Product.Request
{
    public class UpdateProductRoleIdRequest
    {
        public ulong? RoleId { get; set; }
        public ProductName Name { get; set; }
    }
}
