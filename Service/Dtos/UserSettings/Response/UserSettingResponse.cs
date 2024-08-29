using System.Text.Json.Serialization;
using ProPayments.Service.Dtos.UserSettings.Converters;

namespace ProPayments.Service.Dtos.UserSettings.Response;

[JsonConverter(typeof(UserSettingResponseConverter))]
public abstract class UserSettingResponse
{
    public ulong Id { get; set; }
    public string Type { get; set; }
}