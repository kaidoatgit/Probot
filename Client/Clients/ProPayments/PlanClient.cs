using Microsoft.AspNetCore.Http;
using ProPayments.Client.Clients.ProPayments.Dtos;
using ProPayments.Client.Dtos.Plan.Request;
using ProPayments.Client.Dtos.Plan.Response;
using System.Net.Http.Json;

namespace ProPayments.Client.Clients.ProPayments
{
    public class PlanClient
    {
        private readonly HttpClient _httpClient;
        public PlanClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<ApiResponse<IEnumerable<PlanWithOptionsResponse>>> GetPlansWithOptionsAsync()
        {
            ApiResponse<IEnumerable<PlanWithOptionsResponse>> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync("with_options");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<PlanWithOptionsResponse>>();
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
                    Console.WriteLine($"[GetPlansWithOptionsAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetPlansWithOptionsAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }

        public async Task<bool> UpdatePlansRoleIdAsync(IEnumerable<UpdatePlanRoleIdRequest> updateRequest)
        {
            bool apiResponse;
            try
            {
                var response = await _httpClient.PostAsJsonAsync("roleId", updateRequest);
                if (response.IsSuccessStatusCode)
                {
                    apiResponse = true;
                }
                else
                {
                    Console.WriteLine($"[UpdatePlansRoleIdAsync] {response.ReasonPhrase}");
                    apiResponse = false;
                }
            }
            catch (Exception ex)
            {
                string message = $"[UpdatePlansRoleIdAsync] Server Error: {ex.Message}";
                Console.WriteLine(message);
                apiResponse = false;
            }
            return apiResponse;
        }
    }
}
