using System.Text.Json.Serialization;
using Probot.Shared.Dtos.UserSetting.Converters;

namespace Probot.Shared.Dtos.UserSetting.Response;

[JsonConverter(typeof(UserSettingResponseConverter))]
public abstract class UserSettingResponse
{
    public ulong Id { get; set; }
    public string Type { get; set; }
}