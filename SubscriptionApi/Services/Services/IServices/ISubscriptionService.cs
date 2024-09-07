using Probot.Shared.Dtos.Subscription.Request;
using Probot.Shared.Enums;
using Subscription = Probot.Data.Entities.Subscription;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateSubscriptionAsync(SubscriptionRequest request);
        Task<Subscription> ExtendSubscriptionAsync(SubscriptionRequest request);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(ulong userId, ProductName productName);
    }
}
