using Probot.Shared.Enums;

namespace Probot.Shared.Dtos.OAuth.Request;

public class OAuthRequest
{
    public ulong SettingId { get; set; }    
    public string InteractionToken { get; set; } = string.Empty;
    public ulong MessageId { get; set; }
    public RaffleAlertType RaffleAlertType { get; set; }
}
