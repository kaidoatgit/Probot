using Microsoft.AspNetCore.Mvc;
using ProPayments.Service.Dtos.Products.Request;
using ProPayments.Service.Dtos.Products.Response;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Controllers
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

        [HttpGet("with_options")]
        public async Task<IActionResult> GetProductsWithOptionsAsync()
        {
            var productsWithOptions = await _productService.GetProductsWithOptionsAsync();
            return Ok(productsWithOptions);
        }

        [HttpPost("roleId")]
        public async Task<IActionResult> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> request)
        {
            var isModified = await _productService.UpdateProductsRoleIdAsync(request);
            return isModified ? NoContent() : StatusCode(304); //304 = not modified
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

        [HttpGet("options")]
        public async Task<IActionResult> GetProductOptionsAsync()
        {
            var productOptions = await _productService.GetProductOptionsAsync();
            List<ProductOptionResponse> productOptionsResponse = productOptions.Select(p => _mapper.MapToProductOptionResponse(p)).ToList();
            return Ok(productOptionsResponse);
        }
    }
}
