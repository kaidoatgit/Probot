using Microsoft.AspNetCore.Mvc;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProRaffles.Response;
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
        

        [HttpGet("{userId}/pro-raffle")]
        public async Task<IActionResult> GetProRaffleSubscriptionsAsync(ulong userId, [FromQuery] bool isActive)
        {
            var proRaffleSubscriptions = await _subscriptionService.GetSubscriptionsOfTypeAsync<ProRaffle>(userId, isActive);
            IEnumerable<ProRaffleSubscriptionResponse> proRaffleSubscriptionsResponse = proRaffleSubscriptions
                    .Select(prs => _mapper.MapToProRaffleSubscriptionResponse(prs));
            return Ok(proRaffleSubscriptionsResponse);
        }
        // [HttpPost]
        // public async Task<IActionResult> CreateSubscriptionAsync([FromBody] SubscriptionRequest request)
        // {
        //     var subscription = await _subscriptionService.CreateSubscriptionAsync(request);
        //     SubscriptionResponse subscriptionResponse = _mapper.MapToSubscriptionResponse(subscription);
        //     return Ok(subscriptionResponse);
        // }

        // [HttpPost("free")]
        // public async Task<IActionResult> CreateFreeSubscriptionAsync([FromBody] SubscriptionRequest request)
        // {
        //     var subscription = await _subscriptionService.CreateFreeSubscriptionAsync(request);
        //     SubscriptionResponse subscriptionResponse = _mapper.MapToSubscriptionResponse(subscription);
        //     return Ok(subscriptionResponse);
        // }
    }

    public class ProRaffleSubscriptionResponse
    {
        public SubscriptionResponse Subscription { get; set; }
        public ProRaffleResponse ProRaffle { get; set; }
    }
}
