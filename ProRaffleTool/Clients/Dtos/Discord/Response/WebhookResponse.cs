using System.Text.Json.Serialization;

namespace Probot.ProRaffleTool.Clients.Dtos.Discord.Response;

public class WebhookResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}