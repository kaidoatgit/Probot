using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.Order.Request;
using Probot.Shared.Dtos.Order.Response;
using System.Net.Http.Json;

namespace Probot.Client.Clients.SubscriptionApi
{
    public class OrderClient
    {
        private readonly HttpClient _httpClient;
        public OrderClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<ApiResponse<OrderResponse>> CreateOrderAsync(OrderRequest request)
        {
            ApiResponse<OrderResponse> apiResponse = new();
            try
            {
                var response = await _httpClient.PostAsJsonAsync(string.Empty, request);
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<OrderResponse>();
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
                    Console.WriteLine($"[CreateOrderAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateOrderAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
    }
}
