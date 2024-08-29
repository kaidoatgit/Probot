using System;
using System.Text.Json.Serialization;
using ProPayments.Client.Dtos.UserSetting.Converters;

namespace ProPayments.Client.Dtos.UserSetting.Response;

[JsonConverter(typeof(UserSettingResponseConverter))]
public abstract class UserSettingResponse
{
    public ulong Id { get; set; }
}
