using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Probot.Data.Entities;
using Probot.ProRaffleTool.Clients;
using Probot.ProRaffleTool.Dtos.Alphabot.Request;
using Probot.ProRaffleTool.Options;
using Probot.ProRaffleTool.Services.Services.Abstractions;

namespace Probot.ProRaffleTool.Controllers;

[ApiController]
[Route("webhook/alphabot")]
public class AlphabotController : ControllerBase
{
    private readonly ILogger<AlphabotController> _logger;
    private AlphabotSettings _alphabotSettings;
    private readonly IProRaffleSettingService _proRaffleSettingService;
    private readonly AlphabotClient _alphabotClient;
    private readonly IMemoryCache _memoryCache;
    private const string _cacheKey = "pro_raffle_settings";
    private readonly IRaffleRateLimiterService _rateLimiterService;

    public AlphabotController(ILogger<AlphabotController> logger, IOptionsMonitor<AlphabotSettings> alphabotOptions, IProRaffleSettingService proRaffleSettingService, 
        AlphabotClient alphabotClient, IMemoryCache memoryCache, IRaffleRateLimiterService rateLimiterService)
    {
        _logger = logger;

        alphabotOptions.OnChange(updatedSettings =>
        {
            _alphabotSettings = updatedSettings;
        });
        _alphabotSettings = alphabotOptions.CurrentValue;

        _proRaffleSettingService = proRaffleSettingService;
        _alphabotClient = alphabotClient; 
        _memoryCache = memoryCache;
        _rateLimiterService = rateLimiterService;
    }

    [HttpPost("raffles")]
    public async Task<IActionResult> Register([FromBody] RaffleRequest request)
    {
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_alphabotSettings.AlphabotWebhookKey));
        var data = $"{request.Event}\n{request.Timestamp}";
        var hashToCheck = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();

        if (!string.Equals(hashToCheck, request.Hash, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogCritical("Invalid hash found! Request IP address: {IP}", HttpContext.Connection.RemoteIpAddress);
            return BadRequest();
        }

        if(string.Equals(request.Event, "raffle:active", StringComparison.OrdinalIgnoreCase))
        {
            var raffle = request.Data?.Raffle;
            if(raffle == null || string.Equals(raffle.Type, "application", StringComparison.OrdinalIgnoreCase))
            {
                return Ok();
            }
            
            _logger.LogInformation("<WEBHOOK> Active raffle found [{0}] <WEBHOOK>", raffle.Slug);

            if (!_memoryCache.TryGetValue(_cacheKey, out IEnumerable<ProRaffleSetting>? prSettings))
            {
                prSettings = await _proRaffleSettingService.GetSettingsAsync(isPaused: false);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _memoryCache.Set(_cacheKey, prSettings, cacheEntryOptions);
            }

            if(string.Equals(raffle.Type, "fcfs", StringComparison.OrdinalIgnoreCase))
            {
                _ = RegisterRafflesFCFS(prSettings!, raffle.Slug);
            }
            else 
            {
                foreach (var prSetting in prSettings!)
                {
                    _ = RegisterRafflesWithDelay(prSetting, raffle.Slug);
                }
            }  
        }
        return Ok();
    }

    private Task RegisterRafflesFCFS(IEnumerable<ProRaffleSetting> prSettings, string slug)
    {
        foreach (var prSetting in prSettings)
        {
            _ = _alphabotClient.RegisterInRaffleAsync(prSetting, slug);
        }
        return Task.CompletedTask;
    }

    private async Task RegisterRafflesWithDelay(ProRaffleSetting prSetting, string slug)
    {
        var rateLimiter = _rateLimiterService.GetOrAdd(prSetting.Username);
        await rateLimiter.WaitAsync(TimeSpan.FromSeconds(10));
        _ = _alphabotClient.RegisterInRaffleAsync(prSetting, slug);
    }
}
