
using Probot.Data.Entities;
using Probot.Shared.Dtos.ProRaffleSetting.Request;

namespace Probot.SubscriptionApi.Services.Services.IServices;

public interface IProRaffleSettingService
{
    Task<ProRaffleSetting> CreateSettingsAsync(ulong userId, ProRaffleSettingRequest request);
    Task<ProRaffleSetting?> GetSettingAsync(ulong userId, ProRaffleSettingRequest request);
}
