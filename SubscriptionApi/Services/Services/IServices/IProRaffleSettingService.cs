
using Probot.Data.Entities;
using Probot.Shared.Dtos.ProRaffleSetting.Request;

namespace Probot.SubscriptionApi.Services.Services.IServices;

public interface IProRaffleSettingService
{
    Task<ProRaffleSetting> CreateSettingsAsync(ulong userId, ProRaffleSettingRequest request);
    Task<ProRaffleSetting?> GetSettingsAsync(ulong userId, ProRaffleSettingRequest request);
    Task UpdateProRaffleSettingKeyAsync(ulong userId, UpdatePRSettingKeyRequest request);
}
