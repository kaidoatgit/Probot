using Probot.Shared.Dtos.User.Response;
using Probot.Shared.Enums;

namespace Probot.Shared.Dtos.Order.Response;
public class OrderResponse
{
    public ulong Id { get; set; }
    public UserResponse User { get; set; }
    public InvoiceResponse Invoice { get; set; } 
}

public class InvoiceResponse
{
    public ulong Id { get; set; }
    public string PaymentAddress { get; set; }
    public DateTimeOffset OrderExpiryTime { get; set; }
    public decimal TotalAmount { get; set; }
    public Token Token { get; set; }
    public string RecipientAddress { get; set; }
    public List<InvoiceItemResponse> InvoiceItems { get; set; }
}

public class InvoiceItemResponse 
{
    public ProductName ProductName { get; set; }
    public ulong ProductRoleId { get; set; }
    public decimal ProductOptionPrice { get; set; }
    public int ProductOptionPeriod { get; set; }
    public string PeriodDescription { get; set; }
}
