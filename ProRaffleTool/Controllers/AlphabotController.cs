using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Probot.Data;
using Probot.Data.Entities;
using Probot.ProRaffleTool.Clients;
using Probot.ProRaffleTool.Dtos.Alphabot.Request;
using Probot.ProRaffleTool.Options;

namespace Probot.ProRaffleTool.Controllers;

[ApiController]
[Route("webhook/alphabot")]
public class AlphabotController : ControllerBase
{
    private readonly ILogger<AlphabotController> _logger;
    private ProRaffleSettings _proRaffleSettings;
    private readonly ProbotContext _context;
    private readonly AlphabotClient _alphabotClient;
    private readonly IMemoryCache _memoryCache;
    private const string _cacheKey = "user_settings";

    public AlphabotController(ILogger<AlphabotController> logger, IOptionsMonitor<ProRaffleSettings> proRaffleSettings, ProbotContext context, AlphabotClient alphabotClient, IMemoryCache memoryCache)
    {
        _logger = logger;

        proRaffleSettings.OnChange(updatedSettings =>
        {
            _proRaffleSettings = updatedSettings;
        });
        _proRaffleSettings = proRaffleSettings.CurrentValue;

        _context = context;
        _alphabotClient = alphabotClient; 
        _memoryCache = memoryCache;
    }

    [HttpPost("raffles")]
    public async Task<IActionResult> Register([FromBody] RaffleRequest request)
    {
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_proRaffleSettings.AlphabotWebhookKey));
        var data = $"{request.Event}\n{request.Timestamp}";
        var hashToCheck = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();

        if (!string.Equals(hashToCheck, request.Hash, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogCritical("Invalid hash found! Request IP address: {IP}", HttpContext.Connection.RemoteIpAddress);
            return BadRequest();
        }

        if(string.Equals(request.Event, "raffle:active", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("<WEBHOOK> Active raffle found! <WEBHOOK>");
            
            var raffle = request.Data?.Raffle;
            if(raffle == null) return Ok();
            
            if (!_memoryCache.TryGetValue(_cacheKey, out List<ProRaffleSetting>? prSettings))
            {
                prSettings = await _context.ProRaffleSettings
                    .Where(pr => !pr.IsPaused)
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _memoryCache.Set(_cacheKey, prSettings, cacheEntryOptions);
            }
            else
            {
                foreach (var prSetting in prSettings!)
                {
                    // Directly calling the async method
                    _ = _alphabotClient.RegisterInRaffleAsync(prSetting.Key, prSetting.Username, raffle.Slug);
                    // .ContinueWith(task =>
                    // {
                    //     if (task.IsFaulted)
                    //     {
                    //         _logger.LogError(task.Exception, "Error registering raffle for user with username {Username}", prSetting.Username);
                    //     }
                    // });
                }
            }            
        }
        return Ok();
    }
}
