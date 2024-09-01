
using System.ComponentModel.DataAnnotations;
using Probot.Shared.Dtos.UserSetting.Request;

namespace Probot.Shared.Dtos.ProRaffle.Request;

public class ProRaffleRequest : UserSettingRequest
{
    [Required]
    public string AlphabotKey { get; set; }
}
