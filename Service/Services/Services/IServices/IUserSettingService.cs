using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.UserSettings.Request;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface IUserSettingService
    {
        Task<UserSetting> CreateUserSettingAsync(UserSettingRequest request);
    }
}
