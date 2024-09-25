
using IdentityModel.Client;
using Probot.ProRaffleTool.Models;

namespace Probot.ProRaffleTool.Clients.Abstractions;

public interface IDiscordClient
{
    Task<TokenResponse> ExchangeCode(string code);
    Task RevokeAccessToken(string accessToken);
    Task DeleteWebhookAsync(string webhookId, string webhookToken);
    Task EditWebhookMessageAsync(OAuthInteractionData oauthInteractionData, string content);
    Task ExecuteWebhookAsync(string webhookId, string webhookToken, NotificationMessage message);
}
