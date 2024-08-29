using Microsoft.AspNetCore.Mvc;
using ProPayments.Service.Dtos.ProRaffles.Request;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Controllers;

[Route("api/pro_raffles")]
[ApiController]
public class ProRafflesController : ControllerBase
{
    private readonly IProRaffleService _proRaffleService;
    private readonly Mapper _mapper;
    
    public ProRafflesController(IProRaffleService proRaffleService, Mapper mapper)
    {
        _proRaffleService = proRaffleService;
        _mapper = mapper;
    }

    [HttpPatch("{userId}/key")]
    public async Task<IActionResult> UpdateProRaffleKeyAsync(ulong userId, [FromBody] UpdateProRaffleKeyRequest request)
    {
        var isModified = await _proRaffleService.UpdateProRaffleKeyAsync(userId, request);
        return isModified ? NoContent() : StatusCode(304); //304 = not modified
    }
}
