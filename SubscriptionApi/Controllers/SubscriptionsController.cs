using Microsoft.AspNetCore.Mvc;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.Subscription.Response;
using Probot.Shared.Dtos.Subscription.Request;
using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Controllers
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
        
        [HttpPost]
        public async Task<IActionResult> CreateSubscriptionAsync([FromBody] SubscriptionRequest request)
        {
            var subscription = await _subscriptionService.CreateSubscriptionAsync(request);
            SubscriptionResponse subscriptionResponse = _mapper.MapToSubscriptionResponse(subscription);
            return Ok(subscriptionResponse);
        }

        [HttpPost("extend")]
        public async Task<IActionResult> ExtendSubscriptionAsync([FromBody] SubscriptionRequest request)
        {
            var subscription = await _subscriptionService.ExtendSubscriptionAsync(request);
            SubscriptionResponse subscriptionResponse = _mapper.MapToSubscriptionResponse(subscription);
            return Ok(subscriptionResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptionsAsync([FromQuery] ulong userId, [FromQuery] ProductName productName)
        {
            var subscriptions = await _subscriptionService.GetSubscriptionsAsync(userId, productName);
            IEnumerable<SubscriptionResponse> subscriptionsResponse = subscriptions.Select(s => _mapper.MapToSubscriptionResponse(s));
            return Ok(subscriptionsResponse);
        }
    }
}
