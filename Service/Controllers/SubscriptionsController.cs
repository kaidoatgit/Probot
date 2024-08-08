using Microsoft.AspNetCore.Mvc;
using ProPayments.Service.Dtos.Subscriptions.Request;
using ProPayments.Service.Dtos.Subscriptions.Response;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Controllers
{
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly Mapper _mapper;

        public SubscriptionsController(ISubscriptionService subscriptionService, Mapper mapper)
        {
            _subscriptionService = subscriptionService;
            _mapper = mapper;
        }

        [HttpPost("free")]
        public async Task<IActionResult> CreateSubscriptionAsync([FromBody] SubscriptionRequest request)
        {
            var subscription = await _subscriptionService.CreateFreeSubscriptionAsync(request);
            SubscriptionResponse subscriptionResponse = _mapper.MapToSubscriptionResponse(subscription);
            return Ok(subscriptionResponse);
        }
    }
}
