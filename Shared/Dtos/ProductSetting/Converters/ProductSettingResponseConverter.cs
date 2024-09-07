using System.Text.Json;
using System.Text.Json.Serialization;
using Probot.Shared.Dtos.ProRaffleSetting.Response;
using Probot.Shared.Dtos.ProductSetting.Response;

namespace Probot.Shared.Dtos.ProductSetting.Converters;

public class ProductSettingResponseConverter : JsonConverter<ProductSettingResponse>
{
    public override ProductSettingResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Define possible names for the discriminator property
        var possiblePropertyNames = new[] { "Type", "type" };

        // Find the discriminator property
        string type = possiblePropertyNames
            .Select(name => root.TryGetProperty(name, out var prop) ? prop.GetString() : null)
            .FirstOrDefault(value => value != null)
            ?? throw new JsonException("Discriminator property not found.");

        // Deserialize based on the type
        var proRaffleResponse = type switch
        {
            "ProRaffleSetting" => JsonSerializer.Deserialize<ProRaffleSettingResponse>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown type: {type}")
        };
        return proRaffleResponse;
    }

    public override void Write(Utf8JsonWriter writer, ProductSettingResponse value, JsonSerializerOptions options)
    {
        // Serialize the object based on its runtime type
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
