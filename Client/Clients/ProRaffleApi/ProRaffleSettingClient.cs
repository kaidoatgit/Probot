using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Dtos.ProRaffleSetting.Response;
using Probot.Shared.Enums;
using System.Net.Http.Json;
using System.Text.Json;

namespace Probot.Client.Clients.ProRaffleApi;
public class ProRaffleSettingClient
{
    private readonly HttpClient _httpClient;
    public ProRaffleSettingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    internal async Task<ApiResponse<bool>> UpdateProRaffleSettingKeyAsync(ulong userId, UpdatePRSettingKeyRequest request)
    {
        ApiResponse<bool> apiResponse = new();
        try
        {
            var response = await _httpClient.PatchAsync($"{userId}/key", JsonContent.Create(request));
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = true;
            }
            else
            {
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[UpdateProRaffleSettingKeyAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
                apiResponse.Data = false;
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[UpdateProRaffleSettingKeyAsync] {ex.Message}");
        }
        return apiResponse;
    }

    internal async Task<ApiResponse<IEnumerable<ProRaffleSettingResponse>>> GetSettingsAsync(ulong userId)
    {
        ApiResponse<IEnumerable<ProRaffleSettingResponse>> apiResponse = new();
        try
        {
            var response = await _httpClient.GetAsync($"{userId}?isPaused={false}");
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ProRaffleSettingResponse>>();
            }
            else
            {
                var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                Console.WriteLine($"[GetSettingsAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[GetSettingsAsync] {ex.Message}");
        }
        return apiResponse;
    }

    internal async Task<ApiResponse<bool>> DisableAlertAsync(ulong settingId, RaffleAlertType raffleAlertType)
    {
        ApiResponse<bool> apiResponse = new();
        try
        {
            var response = await _httpClient.PatchAsync($"{settingId}/disable_alert", JsonContent.Create(raffleAlertType));
            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = true;
            }
            else
            {
                apiResponse.Data = false;
                var result = await response.Content.ReadAsStringAsync();
                if(!string.IsNullOrEmpty(result))
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(result)!;
                    apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                    apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                }
                else
                {
                    apiResponse.ExceptionResult = ExceptionResult.NotModified304;
                }
                
                Console.WriteLine($"[DisableAlertAsync][{response.StatusCode}] {apiResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
            apiResponse.ErrorMessage = ex.Message;
            Console.WriteLine($"[DisableAlertAsync] {ex.Message}");
        }
        return apiResponse;
    }
   
}