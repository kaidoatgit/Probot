
namespace Probot.ProRaffleTool.Options;

public sealed class OAuthSettings
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string OAuthRedirectUri { get; init; } = string.Empty;
    public string StateKey { get; init; } = string.Empty;
}
