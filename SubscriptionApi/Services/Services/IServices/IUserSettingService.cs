using Probot.Data.Entities;
using Probot.Shared.Dtos.UserSetting.Request;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface IUserSettingService
    {
        Task<(bool, TUserSetting)> GetOrCreateUserSettingAsync<TUserSetting>(UserSettingRequest request) where TUserSetting : UserSetting;
    }
}
