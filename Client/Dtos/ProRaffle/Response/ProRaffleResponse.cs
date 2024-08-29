using ProPayments.Client.Dtos.UserSetting.Response;

namespace ProPayments.Client.Dtos.ProRaffle.Response;

public class ProRaffleResponse : UserSettingResponse
{
    public string Key { get; set; }
    public bool IsPaused { get; set; }
}
