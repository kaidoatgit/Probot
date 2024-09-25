namespace Probot.Client.Models
{
    public class ProductSetting
    {
        public ulong Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class ProRaffleSetting : ProductSetting
    {
        public string Key { get; set; }
        public bool IsPaused { get; set; }
        public bool IsRegisteredAlertEnabled { get; set; }
        public bool IsErrorAlertEnabled { get; set; }
    }
}
