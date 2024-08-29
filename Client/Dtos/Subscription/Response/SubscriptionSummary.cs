namespace ProPayments.Client.Dtos.Subscription.Response;

public class SubscriptionSummary
{
    public ulong UserId { get; set; }
    public int TotalNonActivatedKeys { get; set; }
    public Dictionary<ulong, int> ActiveSubsPerProduct { get; set; } = new();
}

