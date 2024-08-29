using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Order.Response
{
    public class OrderResult
    {        
        public ulong OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ulong UserId { get; set; }
        public Dictionary<ulong, int> TotalKeysByProduct { get; set; } = new();
        // public int TotalProductKeys { get; set; }
        // public List<ulong> ProductRoleIds { get; set; } = new();
    }
}
