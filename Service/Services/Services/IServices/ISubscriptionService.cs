using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.UserSettings.Request;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<Subscription>> GetSubscriptionsOfTypeAsync<TUserSetting>(ulong userId) where TUserSetting : UserSetting;
        Task<Subscription> CreateSubscriptionOfTypeAsync<TUserSetting>(UserSettingRequest request) where TUserSetting : UserSetting;
        Task<Dictionary<ulong, Dictionary<ulong, int>>> GetUsersActiveSubsCountPerProduct(CancellationToken cancellationToken);
    }
}
