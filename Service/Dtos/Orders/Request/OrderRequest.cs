using System.ComponentModel.DataAnnotations;

namespace ProPayments.Service.Dtos.Orders.Request
{
    public class OrderRequest
    {
        [Required]
        public ulong UserId { get; set; }
        [Required]
        public int PlanOptionId { get; set; }
        public bool IsManual { get; set; } = false;
    }
}
