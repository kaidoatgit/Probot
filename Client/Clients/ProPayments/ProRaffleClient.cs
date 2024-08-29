using Microsoft.AspNetCore.Http;
using ProPayments.Client.Clients.ProPayments.Dtos;
using ProPayments.Client.Dtos.ProductKey.Request;
using System.Net.Http.Json;

namespace ProPayments.Client.Clients.ProPayments
{
    public class ProRaffleClient
    {
        private readonly HttpClient _httpClient;
        public ProRaffleClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
    }
}
