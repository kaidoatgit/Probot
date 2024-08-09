namespace ProPayments.Client.Configs
{
    public class AppSettings
    {
        public string Token { get; set; } = string.Empty;
        public ulong SubscriptionChannelId { get; set; }
        public ulong NotificationChannelId { get; set; }
        public ulong BotChannelId { get; set; }
    }

}
