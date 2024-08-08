using System.ComponentModel.DataAnnotations;

namespace ProPayments.Service.Dtos.Orders.Request
{
    public class CompleteOrderRequest
    {
        [Required]
        public ulong OrderId { get; set; }
        [Required]
        public ulong InvoiceId { get; set; }
        [Required]
        public TransactionRequest Transaction { get; set; }
    }

    public class TransactionRequest
    {
        public string Hash { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
