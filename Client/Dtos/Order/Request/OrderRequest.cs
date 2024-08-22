namespace ProPayments.Client.Dtos.Order.Request
{
    public class OrderRequest
    {
        public ulong UserId { get; set; }
        public List<int> ProductOptionsId { get; set; }
    }
}
