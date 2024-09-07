using System.ComponentModel.DataAnnotations;
using Probot.Shared.Dtos.ProductSetting.Request;

namespace Probot.Shared.Dtos.Subscription.Request;

public class SubscriptionRequest
{
    [Required]
    public ulong UserId { get; set; }
    [Required]
    public string Code { get; set; } = string.Empty;
    public ProductSettingRequest? ProductSettingRequest { get; set; }
}
