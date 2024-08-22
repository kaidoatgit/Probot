using ProPayments.Service.Data.Entities;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface ISubscriptionService
    {
        // Task<Subscription> CreateFreeSubscriptionAsync(SubscriptionRequest request);
        // Task<Subscription> CreatePaidSubscriptionAsync(ulong userId, Invoice invoice);
        // Task<Subscription> CreateSubscriptionAsync(SubscriptionRequest request);
        Task CreateSubscriptionAsync(ProductKey productKey, ulong userSettingId);
        Task ExtendSubscriptionAsync(Subscription subscription, ProductKey productKey);
        Task<IEnumerable<Subscription>> GetSubscriptionsOfTypeAsync<TUserSetting>(ulong userId, bool isActive) where TUserSetting : UserSetting;
    }
}
