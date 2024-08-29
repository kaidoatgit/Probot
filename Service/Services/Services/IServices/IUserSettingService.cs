using ProPayments.Service.Data.Entities;
using ProPayments.Service.Data.Entities.Enums;
using ProPayments.Service.Dtos.UserSettings.Request;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface IUserSettingService
    {
        // Task<(bool, UserSetting)> CreateUserSettingAsync(ProductName productName, UserSettingRequest request);
        Task<(bool, TUserSetting)> CreateUserSettingAsync<TUserSetting>(UserSettingRequest request) where TUserSetting : UserSetting;
    }
}
