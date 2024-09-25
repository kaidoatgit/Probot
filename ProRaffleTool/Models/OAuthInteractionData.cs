using Probot.Shared.Enums;

namespace Probot.ProRaffleTool.Models;

public class OAuthInteractionData
{
    public ulong UserId { get; init; }
    public ulong SettingId { get; init; }
    public string InteractionToken { get; init; } = string.Empty;
    public ulong MessageId { get; init; }
    public RaffleAlertType RaffleAlertType { get; init; }
}
