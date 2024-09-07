using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using System.Net.Http.Json;

namespace Probot.Client.Clients.SubscriptionApi
{
    public class ProRaffleSettingClient
    {
        private readonly HttpClient _httpClient;
        public ProRaffleSettingClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<ApiResponse<bool>> UpdateProRaffleSettingKeyAsync(ulong userId, UpdatePRSettingKeyRequest request)
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
                    Console.WriteLine($"[UpdateProRaffleSettingKeyAsync] {response.ReasonPhrase}");
                    apiResponse.Data = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateProRaffleSettingKeyAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
    }
}
