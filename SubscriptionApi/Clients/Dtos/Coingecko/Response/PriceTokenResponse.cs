namespace Probot.SubscriptionApi.Clients.Dtos.Coingecko.Response
{
    public class PriceTokenResponse
    {
        public Solana? Solana { get; set; }
    }

    public class Solana
    {
        public decimal Usd { get; set; } = 0m;
    }
}
