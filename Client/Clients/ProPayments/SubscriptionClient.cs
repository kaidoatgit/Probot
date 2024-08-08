using Microsoft.AspNetCore.Http;
using ProPayments.Client.Clients.ProPayments.Dtos;
using ProPayments.Client.Dtos.Subscription.Request;
using ProPayments.Client.Dtos.Subscription.Response;
using System.Net.Http.Json;

namespace ProPayments.Client.Clients.ProPayments
{
    public class SubscriptionClient
    {
        private readonly HttpClient _httpClient;
        public SubscriptionClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<SubscriptionResponse>> RegisterSubscriptionAsync(SubscriptionRequest request)
        {
            ApiResponse<SubscriptionResponse> apiResponse = new();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("free", request);
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<SubscriptionResponse>();
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
                    Console.WriteLine($"[RegisterSubscriptionAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RegisterSubscriptionAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }

        public async Task<ApiResponse<IEnumerable<UserWithSubscriptions>>> GetUsersWithSubscriptionsAsync()
        {
            ApiResponse<IEnumerable<UserWithSubscriptions>> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync("with_subscriptions");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<UserWithSubscriptions>>();
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
                    Console.WriteLine($"[GetUsersWithSubscriptionsAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUsersWithSubscriptionsAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }

        //public async Task<ApiResponse<CreateSubscriptionResponse>> RegisterSubscriptionAsync(CreateSubscriptionRequest request)
        //{
        //    ApiResponse<CreateSubscriptionResponse> apiResponse = new();
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("free", request);
        //        response.EnsureSuccessStatusCode();
        //        apiResponse.Data = await response.Content.ReadFromJsonAsync<CreateSubscriptionResponse>();
        //    }
        //    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        apiResponse.Result = Result.NotFound;
        //        apiResponse.Message = $"[RegisterSubscriptionAsync] NotFound: {ex.Message}";
        //        Console.WriteLine(apiResponse.Message);
        //    }
        //    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        //    {
        //        apiResponse.Result = Result.Conflict;
        //        apiResponse.Message = $"[RegisterSubscriptionAsync] Conflict: {ex.Message}";
        //        Console.WriteLine(apiResponse.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.Result = Result.InternalServerError;
        //        apiResponse.Message = $"[RegisterSubscriptionAsync] Server Error: {ex.Message}";
        //        Console.WriteLine(apiResponse.Message);
        //    }
        //    return apiResponse;
        //}
    }
}
