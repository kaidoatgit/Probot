namespace ProPayments.Service.Dtos.Subscriptions.Response
{
    public class SubscriptionResponse
    {        
        public ulong UserId { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}