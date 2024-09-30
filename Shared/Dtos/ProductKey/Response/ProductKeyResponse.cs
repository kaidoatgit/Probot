
using Probot.Shared.Dtos.ProductOption.Response;

namespace Probot.Shared.Dtos.ProductKey.Response;

public class ProductKeyResponse
{
    public string Code { get; set; } = null!;
    public ProductOptionResponse ProductOption { get; set; } = null!;
}
