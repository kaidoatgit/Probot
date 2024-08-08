using ProPayments.Service.Clients.Dtos.Coingecko.Response;

namespace ProPayments.Service.Clients.IClients
{
    public interface ICoingeckoClient
    {
        Task<PriceTokenResponse?> GetPriceTokenById(string id);
    }
}