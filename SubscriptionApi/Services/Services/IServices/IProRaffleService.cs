
using Probot.Data.Entities;
using Probot.Shared.Dtos.ProRaffle.Request;

namespace Probot.SubscriptionApi.Services.Services.IServices;

public interface IProRaffleService
{
    Task UpdateProRaffleKeyAsync(ulong userId, UpdateProRaffleKeyRequest request);
    Task<(bool IsNewSetting, ProRaffle ProRaffle)> GetOrCreateSettingsAsync(ProRaffleRequest request);
}
