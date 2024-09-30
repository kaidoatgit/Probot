
using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.User.Request;
public class UserRequest
{
    [Required]
    public ulong Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Username length can't be more than 100.")]
    public string Username { get; set; } 
    public string WalletAddress { get; set; } = string.Empty;
    public string? Email { get; set; }
}
