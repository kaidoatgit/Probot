using ProPayments.Client.Dtos.Subscription.Response;

namespace ProPayments.Client.Dtos.ProRaffle.Response;

public class ProRaffleResponse
{
    public string Key { get; set; }
    public bool IsPaused { get; set; }
    public SubscriptionResponse Subscription { get; set; }
}