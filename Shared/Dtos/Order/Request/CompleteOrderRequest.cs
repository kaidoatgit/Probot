
using System.ComponentModel.DataAnnotations;

namespace Probot.Shared.Dtos.Order.Request;
public class CompleteOrderRequest
{
    [Required]
    public ulong OrderId { get; set; }
    [Required]
    public ulong InvoiceId { get; set; }
    [Required]
    public TransactionRequest? Transaction { get; set; }
}

public class TransactionRequest
{
    public string Hash { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}
