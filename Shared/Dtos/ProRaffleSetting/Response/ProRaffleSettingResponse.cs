
using Probot.Shared.Dtos.ProductSetting.Response;

namespace Probot.Shared.Dtos.ProRaffleSetting.Response;
public class ProRaffleSettingResponse : ProductSettingResponse
{
    public string Key { get; set; }
    public bool IsPaused { get; set; }
}
