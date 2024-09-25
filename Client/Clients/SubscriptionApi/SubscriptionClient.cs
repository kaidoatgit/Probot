using Probot.Shared.Dtos;
using Probot.Shared.Dtos.Subscription.Request;
using Probot.Shared.Dtos.Subscription.Response;
using Newtonsoft.Json;
using Probot.Shared.Enums;
using System.Text;

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
                apiResponse.ErrorMessage = response.ReasonPhrase;

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                if (errorResponse != null)
                {
                    apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                    apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                }
                Console.WriteLine($"[CreateSubscriptionAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[CreateSubscriptionAsync] {ex.Message}");
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
                apiResponse.ErrorMessage = response.ReasonPhrase;

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                if (errorResponse != null)
                {
                    apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                    apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                }
                Console.WriteLine($"[ExtendSubscriptionAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[ExtendSubscriptionAsync] {ex.Message}");
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
                apiResponse.ErrorMessage = response.ReasonPhrase;

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                if (errorResponse != null)
                {
                    apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                    apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                }
                Console.WriteLine($"[GetSubscriptionsAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[GetSubscriptionsAsync] {ex.Message}");
        }

        return apiResponse;
    } 

}
