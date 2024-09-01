using Microsoft.AspNetCore.Mvc;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.Order.Request;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Dtos.ProductKey.Response;

namespace Probot.SubscriptionApi.Controllers
{

    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly Mapper _mapper;

        public OrdersController(IOrderService orderService, Mapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderRequest request)
        {            
            var order = await _orderService.CreateOrderAsync(request);
            OrderResponse orderResponse = _mapper.MapToOrderResponse(order);
            return CreatedAtAction(nameof(GetOrderAsync), new { id = orderResponse.Id }, orderResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderAsync(ulong id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            GetOrderResponse orderResponse = _mapper.MapToGetOrderResponse(order);
            return Ok(orderResponse);
        }

        [HttpGet("{orderId}/status")]
        public async Task<IActionResult> GetOrderStatusAsync(ulong orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            return Ok(new { OrderId = orderId, Status = order.Status.ToString() });
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteOrderAsync([FromBody] CompleteOrderRequest request)
        {
            var productKeys = await _orderService.CompleteOrderAsync(request);
            IEnumerable<ProductKeyResponse> productKeyResponses = productKeys.Select(pk => _mapper.MapToProductKeyResponse(pk));
            return Ok(productKeyResponses);
        }
    }
}
