using Microsoft.AspNetCore.Mvc;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.Product.Request;
using Probot.Shared.Dtos.Product.Response;
using Probot.Shared.Dtos.ProductOption.Response;

namespace Probot.SubscriptionApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly Mapper _mapper;
        public ProductsController(IProductService productService, Mapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            ProductResponse productResponse = _mapper.MapToProductResponse(product);
            return Ok(productResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var products = await _productService.GetProductsAsync();
            List<ProductResponse> productsResponse = products.Select(p => _mapper.MapToProductResponse(p)).ToList();
            return Ok(productsResponse);
        }

        [HttpGet("product_options")]
        public async Task<IActionResult> GetProductsWithOptionsAsync()
        {
            var products = await _productService.GetProductsWithOptionsAsync();
            IEnumerable<ProductResponse> productsResponse = products.Select(p => _mapper.MapToProductResponse(p));
            return Ok(productsResponse);
        }
        
        [HttpPatch("roleId")]
        public async Task<IActionResult> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> request)
        {
            var isModified = await _productService.UpdateProductsRoleIdAsync(request);
            return isModified ? NoContent() : StatusCode(304); //304 = not modified
        }
    
        [HttpGet("options")]
        public async Task<IActionResult> GetProductOptionsAsync()
        {
            var productOptions = await _productService.GetProductOptionsAsync();
            List<ProductOptionResponse> productOptionsResponse = productOptions.Select(p => _mapper.MapToProductOptionResponse(p)).ToList();
            return Ok(productOptionsResponse);
        }
    }
}
