using System.Text.Json;
using System.Text.Json.Serialization;
using ProPayments.Service.Dtos.UserSettings.Response;

namespace ProPayments.Service.Dtos.UserSettings.Converters;

public class UserSettingResponseConverter : JsonConverter<UserSettingResponse>
{
    public override UserSettingResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, UserSettingResponse value, JsonSerializerOptions options)
    {
        // Serialize the object based on its runtime type
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
