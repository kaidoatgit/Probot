namespace Probot.Client.Models
{
    public class UserSetting
    {
        public ulong Id { get; set; }
    }

    public class ProRaffle : UserSetting
    {
        public string Key { get; set; }
        public bool IsPaused { get; set; }
    }
}
