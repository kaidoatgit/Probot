using Newtonsoft.Json;

namespace Probot.Shared.Dtos.Subscription.Response;
public class SubscriptionReminder
{
    public ulong UserId { get; set; }
    public string AlphabotKey { get; set; } = null!;
    public string Username { get; set; } = null!;
    public ulong ProductRoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int DaysLeft { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.None);
    }
}
