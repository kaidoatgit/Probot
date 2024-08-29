
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProRaffles.Request;

namespace ProPayments.Service.Services.Services.IServices;

public interface IProRaffleService
{
    Task<bool> UpdateProRaffleKeyAsync(ulong userId, UpdateProRaffleKeyRequest request);
    Task<(bool IsNewSetting, ProRaffle ProRaffle)> CreateSettingsAsync(ProRaffleRequest request);
}
