using Probot.Shared.Enums;

namespace Probot.Shared.Dtos.Product.Request;
public class UpdateProductRoleIdRequest
{
    public ulong? RoleId { get; set; }
    public ProductName Name { get; set; }
}

