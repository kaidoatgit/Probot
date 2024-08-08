using ProPayments.Service.Clients.Dtos.Coingecko.Response;
using ProPayments.Service.Clients.IClients;

namespace ProPayments.Service.Clients
{
    public class CoingeckoClient : ICoingeckoClient
    {
        private readonly HttpClient _httpClient;

        public CoingeckoClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PriceTokenResponse?> GetPriceTokenById(string id)
        {
            PriceTokenResponse? priceTokenResponse = null;
            try
            {
                var response = await _httpClient.GetAsync($"simple/price?ids={id}&vs_currencies=usd");
                response.EnsureSuccessStatusCode();
                priceTokenResponse = await response.Content.ReadFromJsonAsync<PriceTokenResponse>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return priceTokenResponse;
        }
    }
}
