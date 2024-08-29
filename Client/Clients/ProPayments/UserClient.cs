using Microsoft.AspNetCore.Http;
using ProPayments.Client.Clients.ProPayments.Dtos;
using ProPayments.Client.Dtos.ProductKey.Response;
using ProPayments.Client.Dtos.Subscription.Response;
using ProPayments.Client.Dtos.User.Request;
using ProPayments.Client.Dtos.User.Response;
using System.Net.Http.Json;

namespace ProPayments.Client.Clients.ProPayments
{
    public class UserClient
    {
        private readonly HttpClient _httpClient;
        public UserClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<ApiResponse<IEnumerable<UserSummaryResponse>>> GetUsersWithSummaryAsync()
        {
            ApiResponse<IEnumerable<UserSummaryResponse>> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync("with-summary");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<UserSummaryResponse>>();
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
                    Console.WriteLine($"[GetUsersWithSummaryAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUsersWithSummaryAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
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
        public async Task<ApiResponse<ProductKeyResponse>> GetProductKeyAsync(ulong userId, string code, bool isActivated)
        {
            ApiResponse<ProductKeyResponse> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync($"{userId}/product-keys/{code}?isActivated={isActivated}");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<ProductKeyResponse>();
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
                    Console.WriteLine($"[ProductKeyResponse] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductKeyResponse] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
        public async Task<ApiResponse<IEnumerable<ProductKeyResponse>>> GetProductKeysAsync(ulong userId, bool isActivated)
        {
            ApiResponse<IEnumerable<ProductKeyResponse>> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync($"{userId}/product-keys?isActivated={isActivated}");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductKeyResponse>>();
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
                    Console.WriteLine($"[GetProductKeysAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetProductKeysAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
    }
}
