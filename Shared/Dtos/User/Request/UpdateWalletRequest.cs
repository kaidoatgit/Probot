
using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.User.Request;
public class UpdateWalletRequest
{
    [Required]
    public string WalletAddress { get; set; }
}