using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.UserSetting.Request;
public abstract class UserSettingRequest
{
    [Required]
    public ulong UserId { get; set; }
    [Required]
    public string Code { get; set; }
}

