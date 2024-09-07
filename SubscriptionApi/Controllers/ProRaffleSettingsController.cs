using Microsoft.AspNetCore.Mvc;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.SubscriptionApi.Services.Services.IServices;

namespace Probot.SubscriptionApi.Controllers;

[Route("api/pro_raffle_settings")]
[ApiController]
public class ProRaffleSettingsController : ControllerBase
{
    private readonly IProRaffleSettingService _proRaffleSettingService;
    
    public ProRaffleSettingsController(IProRaffleSettingService proRaffleSettingService)
    {
        _proRaffleSettingService = proRaffleSettingService;
    }

    [HttpPatch("{userId}/key")]
    public async Task<IActionResult> UpdateProRaffleSettingKeyAsync(ulong userId, [FromBody] UpdatePRSettingKeyRequest request)
    {
        await _proRaffleSettingService.UpdateProRaffleSettingKeyAsync(userId, request);
        return NoContent(); //StatusCode(304, "reason"); //304 = not modified
    }
}
