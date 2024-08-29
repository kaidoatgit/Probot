using Microsoft.AspNetCore.Http;
using ProPayments.Client.Clients.ProPayments.Dtos;
using ProPayments.Client.Dtos.Product.Request;
using ProPayments.Client.Dtos.Product.Response;
using System.Net.Http.Json;

namespace ProPayments.Client.Clients.ProPayments
{
    public class ProductClient
    {
        private readonly HttpClient _httpClient;
        public ProductClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetProductsWithOptionsAsync()
        {
            ApiResponse<IEnumerable<ProductResponse>> apiResponse = new();
            try
            {
                var response = await _httpClient.GetAsync("with_options");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductResponse>>();
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
                    Console.WriteLine($"[GetProductsWithOptionsAsync] {apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetProductsWithOptionsAsync] {ex.Message}");
                apiResponse.ErrorMessage = ex.Message;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return apiResponse;
        }
        
        public async Task<bool> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> updateRequest)
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
                    Console.WriteLine($"[UpdateProductsRoleIdAsync] {response.ReasonPhrase}");
                    apiResponse = false;
                }
            }
            catch (Exception ex)
            {
                string message = $"[UpdateProductsRoleIdAsync] Server Error: {ex.Message}";
                Console.WriteLine(message);
                apiResponse = false;
            }
            return apiResponse;
        }
    }
}
