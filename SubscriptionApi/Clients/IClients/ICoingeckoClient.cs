using Probot.SubscriptionApi.Clients.Dtos.Coingecko.Response;

namespace Probot.SubscriptionApi.Clients.IClients;
public interface ICoingeckoClient
{
    Task<PriceTokenResponse?> GetPriceTokenById(string id);
}