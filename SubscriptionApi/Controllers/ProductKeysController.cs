using Microsoft.AspNetCore.Mvc;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.ProductKey.Response;

namespace Probot.SubscriptionApi.Controllers;

[Route("api/product_keys")]
[ApiController]
public class ProductKeysController : ControllerBase
{
    private readonly IProductKeyService _productKeyService;
    private readonly Mapper _mapper;
    public ProductKeysController(IProductKeyService productKeyService, Mapper mapper)
    {
        _productKeyService = productKeyService;
        _mapper = mapper;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetProductKeyAsync(string code, [FromQuery] ulong? userId, [FromQuery] bool? isActivated)
    {
        var productKey = await _productKeyService.GetProductKeyByCodeAsync(code, isActivated);
        if(productKey.UserId != userId)
        {
            return BadRequest($"Product Key with {code} does not belong to the user or does not exist.");
        }
        ProductKeyResponse productKeyResponse = _mapper.MapToProductKeyResponse(productKey);
        return Ok(productKeyResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductKeysAsync([FromQuery] ulong? userId, [FromQuery] bool? isActivated)
    {
        var productKeys = await _productKeyService.GetProductKeysAsync(userId, isActivated);
        IEnumerable<ProductKeyResponse> productKeyResponses = productKeys.Select(pk => _mapper.MapToProductKeyResponse(pk));
        return Ok(productKeyResponses);
    }
}
