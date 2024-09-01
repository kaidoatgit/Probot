using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.ProductKey.Response;

namespace Probot.Client.Clients.SubscriptionApi;

public class ProductKeyClient
{
     private readonly HttpClient _httpClient;
    public ProductKeyClient(HttpClient client)
    {
        _httpClient = client;
    }
    
    public async Task<ApiResponse<ProductKeyResponse>> GetProductKeyAsync(ulong userId, string code, bool isActivated)
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
                Console.WriteLine($"[GetProductKeyAsync] {apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetProductKeyAsync] {ex.Message}");
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
            var response = await _httpClient.GetAsync($"?userId={userId}&isActivated={isActivated}");
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
