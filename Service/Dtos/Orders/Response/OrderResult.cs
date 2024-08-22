using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Dtos.Orders.Response
{
    public class OrderResult
    {
        public ulong OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ulong UserId { get; set; }
        public int TotalProductKeys { get; set; }
        public List<ulong> ProductRoleIds { get; set; } = new();
    }
}
