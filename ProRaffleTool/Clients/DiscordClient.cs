using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Probot.ProRaffleTool.Clients.Abstractions;
using Probot.ProRaffleTool.Exceptions;
using Probot.ProRaffleTool.Models;
using Probot.ProRaffleTool.Options;
using Probot.Shared.Enums;

namespace Probot.ProRaffleTool.Clients;

internal class DiscordClient : IDiscordClient
{
    private readonly HttpClient _httpClient;
    private readonly OAuthSettings _oauthSettings; 

    public DiscordClient(HttpClient httpClient, IOptions<OAuthSettings> oauthOptions)
    {
        _httpClient = httpClient;
        _oauthSettings = oauthOptions.Value;
    }

    public async Task<TokenResponse> ExchangeCode(string code)
    {
        var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
        {
            Address = "oauth2/token",
            Code = code,
            RedirectUri = _oauthSettings.RedirectUri,
            ClientId = _oauthSettings.ClientId,
            ClientSecret = _oauthSettings.ClientSecret
        });

        if (tokenResponse.IsError)
        {
           throw new ProRaffleException(ExceptionResult.FailedDependency424, $"A problem was detected while comunicating with discord. Please try later.");
        }
        return tokenResponse;
    }

    public async Task RevokeAccessToken(string accessToken)
    {
        var tokenResponse = await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
        {
            Address = "oauth2/token/revoke",
            Token = accessToken,
            ClientId = _oauthSettings.ClientId,
            ClientSecret = _oauthSettings.ClientSecret
        });

        if (tokenResponse.IsError)
        {
            throw new ProRaffleException(ExceptionResult.FailedDependency424, $"A problem was detected while comunicating with discord. Please try later.");
        }
    }
    
    public async Task DeleteWebhookAsync(string webhookId, string webhookToken)
    {
        var r = await _httpClient.DeleteAsync($"webhooks/{webhookId}/{webhookToken}");
        Console.WriteLine($"StatusCode: {r.StatusCode} | Reason: {r.ReasonPhrase}");
    }

    public async Task EditWebhookMessageAsync(OAuthInteractionData interactionData, string content)
    {
        var requestUri = $"webhooks/{_oauthSettings.ClientId}/{interactionData.InteractionToken}/messages/{interactionData.MessageId}";
        await _httpClient.PatchAsync(requestUri, JsonContent.Create(new { content = content }));
    }

    public async Task ExecuteWebhookAsync(string webhookId, string webhookToken, NotificationMessage message)
    {
        var embed = new
        {
            description = message.Description,
            color = message.Color,
            footer = new { Text = $"{message.Username}" },
            timestamp = DateTime.UtcNow
        };

        var payload = new
        {
            content = message.IsMentionable ? $"<@{message.UserId}>" : null,
            embeds = new[] { embed },
            allowed_mentions = message.IsMentionable 
                ? new { users = new[] { $"{message.UserId}" } }
                : null
        };

        var endpoint = $"webhooks/{webhookId}/{webhookToken}";
        var response = await _httpClient.PostAsJsonAsync(endpoint, payload);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error sending webhook message: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

    // private async Task GetUserGuilds()
    // {
    //     _httpClient.SetBearerToken("<place here the access token received from the oauth>");
    //     var response = await _httpClient.GetAsync("users/@me/guilds");
    //     var responseString = await response.Content.ReadAsStringAsync();
    // }

    // private async Task AddGuildMember(string guildId, ulong userId)
    // {
    //     _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", "<place here the access token from the bot");
    //     var response = await _httpClient.PostAsJsonAsync($"guilds/{guildId}/members/{userId}", new { access_token = "<place here the access token received from the oauth>" });
    //     var responseString = await response.Content.ReadAsStringAsync();
    // }

    // public async Task ExecuteWebhookAsync(string webhookId, string webhookToken, string description)
    // {
    //     var embed = new
    //     {
    //         title = "Raffle Notification",
    //         description = "You've been entered into the raffle!",
    //         color = 0xffa500, // Orange color
    //         fields = new[]
    //         {
    //             new { name = "Raffle Name", value = "Monthly Giveaway", inline = true },
    //             new { name = "Entries", value = "100", inline = true }
    //         },
    //         footer = new { text = "Good luck!" },
    //         timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
    //     };

    //     var payload = new
    //     {
    //         content = "New raffle notification!",
    //         embeds = new[] { embed }
    //     };

    //     var endpoint = $"webhooks/{webhookId}/{webhookToken}";
    //     var response = await _httpClient.PostAsJsonAsync(endpoint, payload);

    //     if (response.IsSuccessStatusCode)
    //     {
    //         Console.WriteLine("Message sent successfully!");
    //     }
    //     else
    //     {
    //         Console.WriteLine($"Error sending message: {response.StatusCode}");
    //     }
    // }
}
