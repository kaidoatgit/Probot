
using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.ProRaffle.Request;

public class UpdateProRaffleKeyRequest
{
    [Required]
    public string CurrentKey { get; set; }
    [Required]
    public string NewKey { get; set; }
}
