using System.ComponentModel.DataAnnotations;

namespace ProPayments.Service.Dtos.UserSettings.Request
{
    public abstract class UserSettingRequest
    {
        [Required]
        public ulong UserId { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
