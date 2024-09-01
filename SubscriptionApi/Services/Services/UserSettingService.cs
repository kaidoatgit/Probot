using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Data.Entities;
using Probot.Shared.Dtos.ProRaffle.Request;
using Probot.Shared.Dtos.UserSetting.Request;

namespace Probot.SubscriptionApi.Services.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly IProRaffleService _proRaffleService;

        public UserSettingService(IProRaffleService proRaffleService)
        {
            _proRaffleService = proRaffleService;
        }   

        public async Task<(bool, TUserSetting)> GetOrCreateUserSettingAsync<TUserSetting>(UserSettingRequest request)
            where TUserSetting : UserSetting
        {
            bool isNewSetting = false;
            TUserSetting? userSetting = null!;
            
            if (typeof(TUserSetting) == typeof(ProRaffle))
            {
                var result = await _proRaffleService.GetOrCreateSettingsAsync((ProRaffleRequest)request);
                isNewSetting = result.IsNewSetting;
                userSetting = result.ProRaffle as TUserSetting;
            }
            return (isNewSetting, userSetting!);
        }
    }
}
