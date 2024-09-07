using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.Subscription.Request;
using Probot.Shared.Dtos.Subscription.Response;
using Newtonsoft.Json;
using System.Text;
using Probot.Shared.Enums;

namespace Probot.Client.Clients.SubscriptionApi;

public class SubscriptionClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _settings;
    public SubscriptionClient(HttpClient httpClient, JsonSerializerSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<ApiResponse<SubscriptionResponse>> CreateSubscriptionAsync(SubscriptionRequest request)
    {
        ApiResponse<SubscriptionResponse> apiResponse = new();
        try
        {
            var jsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Empty, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                apiResponse.Data = JsonConvert.DeserializeObject<SubscriptionResponse>(jsonResponse, _settings);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var metadata = JsonConvert.DeserializeObject<Metadata>(errorContent);
                
                if (metadata != null)
                {
                    apiResponse.ErrorMessage = metadata.ErrorMessage;
                    apiResponse.ServiceResult = metadata.ServiceResult;
                }
                else
                {
                    apiResponse.StatusCode = (int)response.StatusCode;
                    apiResponse.ErrorMessage = response.ReasonPhrase;
                }

                Console.WriteLine($"[CreateSubscriptionAsync] {apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateSubscriptionAsync] {ex.Message}");
            apiResponse.ErrorMessage = ex.Message;
            apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return apiResponse;
    }

    public async Task<ApiResponse<SubscriptionResponse>> ExtendSubscriptionAsync(SubscriptionRequest request)
    {
        ApiResponse<SubscriptionResponse> apiResponse = new();
        try
        {
            var jsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("extend", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                apiResponse.Data = JsonConvert.DeserializeObject<SubscriptionResponse>(jsonResponse, _settings);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var metadata = JsonConvert.DeserializeObject<Metadata>(errorContent);
                
                if (metadata != null)
                {
                    apiResponse.ErrorMessage = metadata.ErrorMessage;
                    apiResponse.ServiceResult = metadata.ServiceResult;
                }
                else
                {
                    apiResponse.StatusCode = (int)response.StatusCode;
                    apiResponse.ErrorMessage = response.ReasonPhrase;
                }

                Console.WriteLine($"[ExtendSubscriptionAsync] {apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ExtendSubscriptionAsync] {ex.Message}");
            apiResponse.ErrorMessage = ex.Message;
            apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return apiResponse;
    }

    public async Task<ApiResponse<IEnumerable<SubscriptionResponse>>> GetSubscriptionsAsync(ulong userId, ProductName productName)
    {
        ApiResponse<IEnumerable<SubscriptionResponse>> apiResponse = new();
        try
        {
            var response = await _httpClient.GetAsync($"?userId={userId}&productName={productName}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                apiResponse.Data = JsonConvert.DeserializeObject<IEnumerable<SubscriptionResponse>>(jsonResponse, _settings);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var metadata = JsonConvert.DeserializeObject<Metadata>(errorContent);
                
                if (metadata != null)
                {
                    apiResponse.ErrorMessage = metadata.ErrorMessage;
                    apiResponse.ServiceResult = metadata.ServiceResult;
                }
                else
                {
                    apiResponse.StatusCode = (int)response.StatusCode;
                    apiResponse.ErrorMessage = response.ReasonPhrase;
                }

                Console.WriteLine($"[GetSubscriptionsAsync] {apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetSubscriptionsAsync] {ex.Message}");
            apiResponse.ErrorMessage = ex.Message;
            apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return apiResponse;
    } 
}
