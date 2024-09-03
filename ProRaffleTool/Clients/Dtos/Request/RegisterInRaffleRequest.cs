
namespace Probot.ProRaffleTool.Clients.Dtos.Request;

public class RegisterInRaffleRequest
{
    public string Slug { get; set; } = string.Empty;
    public string? MintAddress { get; set; }
    public string? DiscordId { get; set; }
    public string? TwitterId { get; set; }
    public string? TelegramId { get; set; }
    public bool? ExtraEntry { get; set; }
}
