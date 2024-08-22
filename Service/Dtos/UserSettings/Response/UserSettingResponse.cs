using System.Text.Json.Serialization;
using ProPayments.Service.Dtos.UserSettings.Converters;
using ProPayments.Service.Dtos.Subscriptions.Response;

namespace ProPayments.Service.Dtos.UserSettings.Response;

[JsonConverter(typeof(UserSettingResponseConverter))]
public abstract class UserSettingResponse
{
    public ulong Id { get; set; }
    public ulong SubscriptionId { get; set; }
    public SubscriptionResponse Subscription { get; set; }
    public abstract string Type { get; }
}