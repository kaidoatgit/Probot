using System.Text.Json;
using IdentityModel.Client;
using Probot.ProRaffleTool.Clients.Dtos.Discord.Response;

namespace Probot.ProRaffleTool.Mappers;

public class CustomMapper
{
    public static WebhookResponse MapToWebhookResponse(TokenResponse tokenResponse)
    {
        var jsonDoc = JsonDocument.Parse(tokenResponse.Raw!);
        var webhookJson = jsonDoc.RootElement.GetProperty("webhook").GetRawText();
        return JsonSerializer.Deserialize<WebhookResponse>(webhookJson)!;
    }
}
    
