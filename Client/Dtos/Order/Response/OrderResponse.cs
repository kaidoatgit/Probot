using ProPayments.Client.Dtos.User.Response;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Order.Response
{
    public class OrderResponse
    {
        public ulong Id { get; set; }
        public UserResponse User { get; set; }
        public InvoiceResponse Invoice { get; set; }

    }

    public class InvoiceResponse
    {
        //Invoice details
        public ulong Id { get; set; }

        //User details
        public string PaymentAddress { get; set; }

        //Order details
        public DateTimeOffset OrderExpiryTime { get; set; }

        //Transaction details
        public decimal TotalAmount { get; set; }
        public Token Token { get; set; }
        public string RecipientAddress { get; set; }

        public List<InvoiceItemResponse> InvoiceItems { get; set; }
    }

    public class InvoiceItemResponse 
    {
        public PlanType PlanType { get; set; }
        public ulong PlanRoleId { get; set; }
        public decimal PlanOptionPrice { get; set; }
        public int PlanOptionPeriod { get; set; }
        public string PeriodDescription { get; set; }
    }
}
