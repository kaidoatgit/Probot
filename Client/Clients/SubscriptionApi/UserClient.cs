using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.User.Request;
using Probot.Shared.Dtos.User.Response;
using Probot.Shared.Enums;
using System.Net.Http.Json;

namespace Probot.Client.Clients.SubscriptionApi;
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
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[RegisterUserAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[RegisterUserAsync] {ex.Message}");
        }
        return apiResponse;
    }

    public async Task<ApiResponse<bool>> UpdateWalletAsync(ulong userId, string walletAddress)
    {
        ApiResponse<bool> apiResponse = new();
        try
        {
            var request = new UpdateWalletRequest { WalletAddress = walletAddress };
            var response = await _httpClient.PatchAsync($"{userId}/wallet_address", JsonContent.Create(request));
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = true;
            }
            else
            {
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[UpdateWalletAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[UpdateWalletAsync] {ex.Message}");
        }
        return apiResponse;
    }
    
    public async Task<ApiResponse<IEnumerable<UserMetricsResponse>>> GetUsersWithMetricsAsync()
    {
        ApiResponse<IEnumerable<UserMetricsResponse>> apiResponse = new();
        try
        {
            var response = await _httpClient.GetAsync("metrics");
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<UserMetricsResponse>>();
            }
            else
            {
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[GetUsersWithMetricsAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[GetUsersWithMetricsAsync] {ex.Message}");
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
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[GetUserAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[GetUserAsync] {ex.Message}");
        }
        return apiResponse;
    }
}
