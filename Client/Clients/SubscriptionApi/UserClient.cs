using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.User.Request;
using Probot.Shared.Dtos.User.Response;
using System.Net.Http.Json;

namespace Probot.Client.Clients.SubscriptionApi
{
    public class UserClient
    {
        private readonly HttpClient _httpClient;
        public UserClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<ApiResponse<UserResponse>> RegisterUserAsync(UserRequest request)
        {
            ApiResponse<UserResponse> apiResponse = new();
            try
            {
                var response = await _httpClient.PostAsJsonAsync(string.Empty, request);
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<UserResponse>();
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
                    Console.WriteLine($"[RegisterUserAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RegisterUserAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> UpdateWalletAsync(ulong userId, string userAddressWallet)
        {
            ApiResponse<bool> apiResponse = new();
            try
            {
                var request = new { WalletAddress = userAddressWallet };
                var response = await _httpClient.PutAsJsonAsync($"{userId}/wallet", request);
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = true;
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
                    Console.WriteLine($"[UpdateWalletAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateWalletAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
        
        public async Task<ApiResponse<IEnumerable<UserMetricsResponse>>> GetUsersWithMetricsAsync()
        {
            ApiResponse<IEnumerable<UserMetricsResponse>> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync("with-metrics");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<UserMetricsResponse>>();
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
                    Console.WriteLine($"[GetUsersWithMetricsAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUsersWithMetricsAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
    
        public async Task<ApiResponse<UserResponse>> GetUserAsync(ulong userId)
        {
            ApiResponse<UserResponse> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync($"{userId}");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<UserResponse>();
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
                    Console.WriteLine($"[GetUserAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
    
    }
}
