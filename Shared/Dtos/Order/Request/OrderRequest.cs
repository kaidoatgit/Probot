
using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.Order.Request;
public class OrderRequest
{
    [Required]
    public ulong UserId { get; set; }
    [Required]
    public List<int> ProductOptionsId { get; set; } = new();
    public bool IsManual { get; set; } = false;
}
