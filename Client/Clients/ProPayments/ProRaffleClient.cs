using Microsoft.AspNetCore.Http;
using ProPayments.Client.Clients.ProPayments.Dtos;
using ProPayments.Client.Dtos.ProductKey.Request;
using ProPayments.Client.Dtos.ProRaffle.Response;
using ProPayments.Client.Dtos.ProRaffle.Request;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProPayments.Client.Clients.ProPayments
{
    public class ProRaffleClient
    {
        private readonly HttpClient _httpClient;
        public ProRaffleClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<ProRaffleResponse>> CreateProRaffleAsync(ProRaffleRequest request)
        {
            ApiResponse<ProRaffleResponse> apiResponse = new();
            try
            {
                var response = await _httpClient.PostAsJsonAsync(string.Empty, request);
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<ProRaffleResponse>();
                }
                else
                {
                    var metadata = await response.Content.ReadFromJsonAsync<Metadata>();
                    if (metadata != null && metadata.StatusCode != 0)
                    {
                        apiResponse.StatusCode = metadata.StatusCode;
                        apiResponse.ErrorMessage = metadata.ErrorMessage;
                    }
                    else
                    {
                        apiResponse.StatusCode = (int)response.StatusCode;
                        apiResponse.ErrorMessage = response.ReasonPhrase;
                    }
                    Console.WriteLine($"[CreateProRaffleAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateProRaffleAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> UpdateProRaffleKeyAsync(ulong userId, UpdateProRaffleKeyRequest request)
        {
            ApiResponse<bool> apiResponse = new();
            try
            {
                var response = await _httpClient.PatchAsync($"{userId}/key", JsonContent.Create(request));
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = true;
                }
                else
                {
                    Console.WriteLine($"[UpdateProRaffleKeyAsync] {response.ReasonPhrase}");
                    apiResponse.Data = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateProRaffleKeyAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }

        public async Task<ApiResponse<IEnumerable<ProRaffleResponse>>> GetProRaffleSubscriptionsAsync(ulong userId, bool isActive)
        {
            ApiResponse<IEnumerable<ProRaffleResponse>> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync($"{userId}/pro-raffle-subscriptions?isActive={isActive}");
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = {  new ProRaffleResponseConverter() }
                    };
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ProRaffleResponse>>(options);
                }
                else
                {
                    var metadata = await response.Content.ReadFromJsonAsync<Metadata>();
                    if (metadata != null && metadata.StatusCode != 0)
                    {
                        apiResponse.StatusCode = metadata.StatusCode;
                        apiResponse.ErrorMessage = metadata.ErrorMessage;
                    }
                    else
                    {
                        apiResponse.StatusCode = (int)response.StatusCode;
                        apiResponse.ErrorMessage = response.ReasonPhrase;
                    }
                    Console.WriteLine($"[GetProRaffleSubscriptions] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetProRaffleSubscriptions] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
    }
}
