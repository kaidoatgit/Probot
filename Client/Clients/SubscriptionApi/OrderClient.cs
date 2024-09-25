using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.Order.Request;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Enums;
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
                    var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                    apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                    apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                    Console.WriteLine($"[CreateOrderAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
                apiResponse.ErrorMessage = ex.Message;
                Console.WriteLine($"[CreateOrderAsync] {ex.Message}");
            }
            return apiResponse;
        }
    }
}
