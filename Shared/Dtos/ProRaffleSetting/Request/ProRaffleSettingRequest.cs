
using System.ComponentModel.DataAnnotations;
using Probot.Shared.Dtos.ProductSetting.Request;

namespace Probot.Shared.Dtos.ProRaffleSetting.Request;

public class ProRaffleSettingRequest : ProductSettingRequest
{
    [Required]
    public string AlphabotKey { get; set; } = string.Empty;
}
