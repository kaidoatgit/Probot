using Probot.Data.Entities;
using Probot.ProRaffleTool.Clients.Dtos.Discord.Response;
using Probot.ProRaffleTool.Models;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Enums;

namespace Probot.ProRaffleTool.Services.Services.Abstractions;

public interface IProRaffleSettingService
{    
    Task<ProRaffleSetting?> GetSettingAsync(ulong settingId, bool dbTracking = false);
    Task<IEnumerable<ProRaffleSetting>> GetSettingsAsync(ulong? userId = null, bool? isPaused = null);
    Task UpdateProRaffleSettingKeyAsync(ulong userId, UpdatePRSettingKeyRequest request);
    Task EnableAlertAsync(OAuthInteractionData interactionData, WebhookResponse webhookResponse);
    Task<bool> DisableAlertAsync(ulong settingId, RaffleAlertType raffleAlertType);
}
