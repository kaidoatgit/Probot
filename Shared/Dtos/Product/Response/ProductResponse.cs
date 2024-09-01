using Probot.Shared.Dtos.ProductOption.Response;
using Probot.Shared.Enums;

namespace Probot.Shared.Dtos.Product.Response;
public class ProductResponse
{
    public int Id { get; set; }
    public ulong? RoleId { get; set; }
    public ProductName Name { get; set; }
    public string? Description { get; set; }
    public List<ProductOptionResponse>? ProductOptions { get; set; }
}
