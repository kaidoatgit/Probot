using Probot.Data.Entities;
using Probot.Shared.Dtos.UserSetting.Request;
using Subscription = Probot.Data.Entities.Subscription;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateSubscriptionOfTypeAsync<TUserSetting>(UserSettingRequest request) where TUserSetting : UserSetting;
        Task<IEnumerable<Subscription>> GetSubscriptionsOfTypeAsync<TUserSetting>(ulong userId) where TUserSetting : UserSetting;
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(bool? onlyActives = null);
    }
}
