
namespace Probot.Shared.Dtos.ProductSetting.Response;

// [JsonConverter(typeof(ProductSettingResponseConverter))]
public abstract class ProductSettingResponse
{
    public ulong Id { get; set; }
    public string Discriminator { get; set; }
    public string Username { get; set; } = string.Empty;

    protected ProductSettingResponse()
    {
        Discriminator = GetType().Name;
    }
}