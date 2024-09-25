using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Probot.ProRaffleTool.Attributes;
using Probot.ProRaffleTool.Clients.Abstractions;
using Probot.ProRaffleTool.Clients.Dtos.Discord.Response;
using Probot.ProRaffleTool.Mappers;
using Probot.ProRaffleTool.Models;
using Probot.ProRaffleTool.Options;
using Probot.ProRaffleTool.Services.Services.Abstractions;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.OAuth.Request;
using Probot.Shared.Enums;
using Probot.Shared.Helpers;

namespace Probot.ProRaffleTool.Controllers;

[Route("api/oauth2/discord")]
[RequiresApiKey]
[ApiController]
public class OAuthDiscordController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly ITokenService _tokenService;
    private readonly IProRaffleSettingService _proRaffleSettingService;
    private readonly OAuthSettings _oauthSettings; 
    private readonly IDiscordClient _discordClient;
    private readonly Mapper _mapper;
    public OAuthDiscordController(IMemoryCache memoryCache, ITokenService tokenService, IProRaffleSettingService proRaffleSettingService,
        IOptions<OAuthSettings> oauthOptions, IDiscordClient discordClient, Mapper mapper)
    {
        _memoryCache = memoryCache;
        _tokenService = tokenService;
        _proRaffleSettingService = proRaffleSettingService;
        _oauthSettings = oauthOptions.Value;
        _discordClient = discordClient;
        _mapper = mapper;
    }

    [HttpPost("url")]
    public async Task<IActionResult> CreateOAuthUrlAsync([FromBody] OAuthRequest request)
    {
        var setting = await _proRaffleSettingService.GetSettingAsync(request.SettingId);
        if(setting == null)
        {
            return NotFound(new ErrorResponse
            {
                ExceptionResult = ExceptionResult.ProductSettingNotFound404,
                ErrorMessage = $"Product settings with settingId:{request.SettingId} not found."
            });
        }

        var (token, hashedToken) = _tokenService.CreateTokens();
        var baseUri = "https://discord.com/oauth2/authorize";
        var @params = new Dictionary<string, string?>
        {
            { "response_type", "code" },
            { "client_id", _oauthSettings.ClientId },
            { "scope", "webhook.incoming" },
            { "state", $"{token}" },
            { "redirect_uri", _oauthSettings.OAuthRedirectUri }
        };
        string oauthUri = QueryHelpers.AddQueryString(baseUri, @params);
        
        var interactionData = new OAuthInteractionData
        {
            UserId = setting.UserId,
            SettingId = setting.Id,
            InteractionToken = request.InteractionToken,
            MessageId = request.MessageId,
            RaffleAlertType = request.RaffleAlertType
        };
        _memoryCache.Set(hashedToken, interactionData, TimeSpan.FromMinutes(5)); 

        return Ok(oauthUri);
    }

    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        string cacheKey = _tokenService.CreateTokens(state).hashedToken;
        if (!_memoryCache.TryGetValue(cacheKey, out OAuthInteractionData? oauthInteractionData) || oauthInteractionData == null)
        {
            return BadRequest("The request is invalid.");
        }
        _memoryCache.Remove(cacheKey);

        TokenResponse tokenResponse = await _discordClient.ExchangeCode(code);
        WebhookResponse webhookResponse = CustomMapper.MapToWebhookResponse(tokenResponse);       
        await _proRaffleSettingService.EnableAlertAsync(oauthInteractionData, webhookResponse);

        var settings = await _proRaffleSettingService.GetSettingsAsync(oauthInteractionData.UserId, isPaused: false);
        var settingsResponse = settings.Select(prs => _mapper.MapToProRaffleSettingResponse(prs)).ToList();   
        string content = ProRaffleHelper.CreateNotificationMessageContent(settingsResponse); 
        await _discordClient.EditWebhookMessageAsync(oauthInteractionData, content.ToString());

        return Ok("Notification activated successfully. You may start receiving notifications within 10 minutes.");
    }

    [HttpPost("revoke/access-token")]
    public async Task<IActionResult> Revoke([FromBody] string accessToken)
    {
        await _discordClient.RevokeAccessToken(accessToken);
        return NoContent();
    }
}