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
        var productKey = await _productKeyService.GetProductKeyByCodeAsync(code, userId: userId, isActivated: isActivated);
        ProductKeyResponse productKeyResponse = _mapper.MapToProductKeyResponse(productKey);
        return Ok(productKeyResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductKeysAsync([FromQuery] ulong? userId, [FromQuery] bool? isActivated)
    {
        var productKeys = await _productKeyService.GetProductKeysAsync(userId, isActivated);
        IEnumerable<ProductKeyResponse> productKeysResponse = productKeys.Select(pk => _mapper.MapToProductKeyResponse(pk));
        return Ok(productKeysResponse);
    }

    [HttpPatch("{code}/claim")]
    public async Task<IActionResult> ClaimProductKeyAsync(string code, [FromBody] ulong userId)
    {
        var productKey = await _productKeyService.ClaimProductKeyAsync(code, userId);
        ProductKeyResponse productKeyResponse = _mapper.MapToProductKeyResponse(productKey);
        return Ok(productKeyResponse);
    }

    [HttpPost("test/7days")]
    public async Task<IActionResult> CreateTestingKeys7DaysAsync([FromBody] int amount)
    {
        return Ok(await _productKeyService.GenerateProductKeysAsync(amount, 7));
    }

    [HttpPost("test/15days")]
    public async Task<IActionResult> CreateTestingKeys15DaysAsync([FromBody] int amount)
    {
        return Ok(await _productKeyService.GenerateProductKeysAsync(amount, 15));
    }

}
