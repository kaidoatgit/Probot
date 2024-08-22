
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProRaffles.Request;
using ProPayments.Service.Dtos.UserSettings.Request;

namespace ProPayments.Service.Services.Services.IServices;

public interface IProRaffleService
{
    Task<bool> UpdateProRaffleKeyAsync(ulong userId, UpdateProRaffleKeyRequest request);
    Task<IEnumerable<ProRaffle>> GetProRaffleSubscriptionsAsync(ulong userId, bool isActive);
    Task<UserSetting> CreateProRaffleAsync(UserSettingRequest request);
}
