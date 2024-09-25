using Microsoft.AspNetCore.Mvc;
using Probot.ProRaffleTool.Attributes;
using Probot.ProRaffleTool.Mappers;
using Probot.ProRaffleTool.Services.Services.Abstractions;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Dtos.ProRaffleSetting.Response;
using Probot.Shared.Enums;

namespace Probot.ProRaffleTool.Controllers;

[Route("api/pro_raffle_settings")]
[RequiresApiKey]
[ApiController]
public class ProRaffleSettingsController : ControllerBase
{
    private readonly IProRaffleSettingService _proRaffleSettingService;
    private readonly Mapper _mapper;
    
    public ProRaffleSettingsController(IProRaffleSettingService proRaffleSettingService, Mapper mapper)
    {
        _proRaffleSettingService = proRaffleSettingService;
        _mapper = mapper;
    }
    
    [HttpPatch("{userId}/key")]
    public async Task<IActionResult> UpdateProRaffleSettingKeyAsync(ulong userId, [FromBody] UpdatePRSettingKeyRequest request)
    {
        await _proRaffleSettingService.UpdateProRaffleSettingKeyAsync(userId, request);
        return NoContent();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetSettingsAsync(ulong userId, [FromQuery] bool? isPaused = null)
    {
        var settings = await _proRaffleSettingService.GetSettingsAsync(userId, isPaused);
        List<ProRaffleSettingResponse> settingsResponse = settings.Select(prs => _mapper.MapToProRaffleSettingResponse(prs)).ToList();
        return Ok(settingsResponse);
    }

    [HttpPatch("{settingId}/disable_alert")]
    public async Task<IActionResult> DisableAlertAsync(ulong settingId, [FromBody] RaffleAlertType raffleAlertType)
    {
        var isDisabled = await _proRaffleSettingService.DisableAlertAsync(settingId, raffleAlertType);
        return isDisabled ? NoContent() : StatusCode(304);
    }
}
