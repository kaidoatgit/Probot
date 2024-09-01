using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.ProRaffle.Request;
using Probot.Shared.Dtos.Subscription.Response;
using Probot.Shared.Dtos.UserSetting.Converters;

namespace Probot.Client.Clients.SubscriptionApi;

public class SubscriptionClient
{
    private readonly HttpClient _httpClient;
    public SubscriptionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<SubscriptionResponse>> CreateProRaffleSubscriptionAsync(ProRaffleRequest request)
    {
        ApiResponse<SubscriptionResponse> apiResponse = new();
        try
        {
            var response = await _httpClient.PostAsJsonAsync("pro-raffle", request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = {  new UserSettingResponseConverter() }
                };
                apiResponse.Data = await response.Content.ReadFromJsonAsync<SubscriptionResponse>(options);
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
                Console.WriteLine($"[CreateProRaffleSubscriptionAsync] {apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateProRaffleSubscriptionAsync] {ex.Message}");
            apiResponse.ErrorMessage = ex.Message;
            apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
        }
        return apiResponse;
    }

    public async Task<ApiResponse<IEnumerable<SubscriptionResponse>>> GetProRaffleSubscriptionsAsync(ulong userId)
    {
        ApiResponse<IEnumerable<SubscriptionResponse>> apiResponse = new();
        try
        {
            var response = await _httpClient.GetAsync($"{userId}/pro-raffle");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = {  new UserSettingResponseConverter() }
                };
                apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<SubscriptionResponse>>(options);
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
                Console.WriteLine($"[GetProRaffleSubscriptionsAsync] {apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetProRaffleSubscriptionsAsync] {ex.Message}");
            apiResponse.ErrorMessage = ex.Message;
            apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
        }
        return apiResponse;
    } 
}
