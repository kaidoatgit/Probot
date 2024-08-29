using ProPayments.Service.Dtos.UserSettings.Response;

namespace ProPayments.Service.Dtos.ProRaffles.Response;
public class ProRaffleResponse : UserSettingResponse
{
    public string Key { get; set; }
    public bool IsPaused { get; set; }
}
