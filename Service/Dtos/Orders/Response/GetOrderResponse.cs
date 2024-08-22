using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Dtos.Orders.Response
{
    public class GetOrderResponse
    {
        public ulong Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTimeOffset ExpiryTime { get; set; }
        public OrderStatus Status { get; set; }

        public ulong UserId { get; set; }
        public ulong TransactionId { get; set; }
        public ulong InvoiceId { get; set; }
        public ulong? SubscriptionId { get; set; }
    }
}
