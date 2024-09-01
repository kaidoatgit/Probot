using Microsoft.AspNetCore.Mvc;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.ProRaffle.Request;

namespace Probot.SubscriptionApi.Controllers;

[Route("api/pro_raffles")]
[ApiController]
public class ProRafflesController : ControllerBase
{
    private readonly IProRaffleService _proRaffleService;
    
    public ProRafflesController(IProRaffleService proRaffleService)
    {
        _proRaffleService = proRaffleService;
    }

    [HttpPatch("{userId}/key")]
    public async Task<IActionResult> UpdateProRaffleKeyAsync(ulong userId, [FromBody] UpdateProRaffleKeyRequest request)
    {
        await _proRaffleService.UpdateProRaffleKeyAsync(userId, request);
        return NoContent(); //StatusCode(304, "reason"); //304 = not modified
    }
}
