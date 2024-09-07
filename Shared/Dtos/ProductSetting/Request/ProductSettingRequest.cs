using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.ProductSetting.Request;
public abstract class ProductSettingRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    public string Discriminator { get; set; } 
    protected ProductSettingRequest()
    {
        Discriminator = GetType().Name;;
    }
}
