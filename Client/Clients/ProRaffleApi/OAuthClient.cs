using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.OAuth.Request;
using Probot.Shared.Enums;

namespace Probot.Client.Clients.ProRaffleApi;

public class OAuthClient
{
    private readonly HttpClient _httpClient;
    public OAuthClient(HttpClient client)
    {
        _httpClient = client;
    }
    public async Task<ApiResponse<string>> CreateOAuthUrlAsync(OAuthRequest request)
    {
        ApiResponse<string> apiResponse = new();
        try
        {
            var response = await _httpClient.PostAsJsonAsync("url", request);
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[CreateOAuthUrlAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[CreateOAuthUrlAsync] {ex.Message}");
        }
        return apiResponse;
    }

}
