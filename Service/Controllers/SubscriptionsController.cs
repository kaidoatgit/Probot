using Microsoft.AspNetCore.Mvc;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProRaffles.Request;
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
        
        [HttpPost("pro-raffle")]
        public async Task<IActionResult> CreateProRaffleSubscriptionAsync([FromBody] ProRaffleRequest request)
        {
            var subscription = await _subscriptionService.CreateSubscriptionOfTypeAsync<ProRaffle>(request);
            SubscriptionResponse subscriptionResponse = _mapper.MapToSubscriptionResponse(subscription);
            return Ok(subscriptionResponse);
        }

        [HttpGet("{userId}/pro-raffle")]
        public async Task<IActionResult> GetProRaffleSubscriptionsAsync(ulong userId)
        {
            var subscriptions = await _subscriptionService.GetSubscriptionsOfTypeAsync<ProRaffle>(userId);
            IEnumerable<SubscriptionResponse> subscriptionsResponse = subscriptions
                .Select(s => _mapper.MapToSubscriptionResponse(s));
            return Ok(subscriptionsResponse);
        }
    }
}
