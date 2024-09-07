
using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.ProRaffleSetting.Request;

public class UpdatePRSettingKeyRequest
{
    [Required]
    public string CurrentKey { get; set; }
    [Required]
    public string NewKey { get; set; }
}
