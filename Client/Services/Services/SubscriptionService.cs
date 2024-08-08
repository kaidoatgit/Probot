using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Dtos.Subscription.Request;
using ProPayments.Client.Exceptions;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models;

namespace ProPayments.Client.Services.Services
{
    public class SubscriptionService
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly Mapper _mapper;
        public SubscriptionService(SubscriptionClient subscriptionClient, Mapper mapper)
        {
            _subscriptionClient = subscriptionClient;
            _mapper = mapper;
            Console.WriteLine("Subscription Manager created");
        }

        public async Task<Subscription> CreateSubscriptionAsync(ulong userId, Plan plan, int period)
        {
            int planOptionId = plan.GetPlanOptionId(period);
            var subscriptionRequest = new SubscriptionRequest
            {
                UserId = userId,
                PlanOptionId = planOptionId
            };
            var apiResponse = await _subscriptionClient.RegisterSubscriptionAsync(subscriptionRequest);
            if (apiResponse.Data == null)
            {
                throw new SubscriptionException(apiResponse.StatusCode, string.Empty);
            }
            else
            {
                return _mapper.MapToSubscription(apiResponse.Data);
            }
        }

        //public async Task<ManagerResponse<Subscription>> CreateSubscriptionAsync(ulong userId, Plan plan, int period)
        //{
        //    int planOptionId = plan.GetPlanOptionId(period);
        //    var createSubscriptionDto = new CreateSubscriptionRequest()
        //    {
        //        UserId = userId,
        //        PlanOptionId = planOptionId
        //    };
        //    var apiResponse = await _subscriptionClient.RegisterSubscriptionAsync(createSubscriptionDto);

        //    var response = _mapper.MapToSubscription(apiResponse);
        //    if (response.Data != null)
        //    {
        //        response.Result = Result.Success;
        //    }
        //    return response;
        //}
    }
}
