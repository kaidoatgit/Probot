using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.ProductKey.Response;
using Probot.Shared.Enums;

namespace Probot.Client.Clients.SubscriptionApi;

public class ProductKeyClient
{
    private readonly HttpClient _httpClient;
    public ProductKeyClient(HttpClient client)
    {
        _httpClient = client;
    }
    
    public async Task<ApiResponse<ProductKeyResponse>> GetProductKeyAsync(string code, ulong userId, bool isActivated)
    {
        ApiResponse<ProductKeyResponse> apiResponse = new();
        try
        {
            var response = await _httpClient.GetAsync($"{code}?userId={userId}&isActivated={isActivated}");
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = await response.Content.ReadFromJsonAsync<ProductKeyResponse>();
            }
            else
            {
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[GetProductKeyAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[GetProductKeyAsync] {ex.Message}");
        }
        return apiResponse;
    }

    public async Task<ApiResponse<IEnumerable<ProductKeyResponse>>> GetProductKeysAsync(ulong userId, bool isActivated)
    {
        ApiResponse<IEnumerable<ProductKeyResponse>> apiResponse = new();
        try
        {
            var response = await _httpClient.GetAsync($"?userId={userId}&isActivated={isActivated}");
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductKeyResponse>>();
            }
            else
            {
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[GetProductKeysAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[GetProductKeysAsync] {ex.Message}");
        }
        return apiResponse;
    } 
}
