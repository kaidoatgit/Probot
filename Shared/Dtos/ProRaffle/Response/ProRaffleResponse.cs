
using Probot.Shared.Dtos.UserSetting.Response;

namespace Probot.Shared.Dtos.ProRaffle.Response;
public class ProRaffleResponse : UserSettingResponse
{
    public string Key { get; set; }
    public bool IsPaused { get; set; }
}
