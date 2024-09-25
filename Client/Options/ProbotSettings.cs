namespace Probot.Client.Options;

internal class ProbotSettings
{
    public string BotToken { get; init; } = string.Empty;
    public ulong SubscriptionChannelId { get; init; }
    public ulong NotificationChannelId { get; init; }
    public string ProRaffleApiKey { get; init; } = string.Empty;
    public ulong ProRaffleChannelId { get; init; }
}