namespace ProPayments.Client.Dtos.Order.Request
{
    public class OrderRequest
    {
        public ulong UserId { get; set; }
        public List<int> PlanOptionsId { get; set; }
    }
}
