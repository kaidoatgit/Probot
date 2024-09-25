using Microsoft.AspNetCore.Http;
using Probot.Shared.Dtos;
using Probot.Shared.Dtos.Product.Request;
using Probot.Shared.Dtos.Product.Response;
using Probot.Shared.Enums;
using System.Net.Http.Json;

namespace Probot.Client.Clients.SubscriptionApi
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
                var response = await _httpClient.GetAsync("product_options");
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductResponse>>();
                }
                else
                {
                    var errorResponse = (await response.Content.ReadFromJsonAsync<ErrorResponse>())!;
                    apiResponse.ExceptionResult = errorResponse.ExceptionResult;
                    apiResponse.ErrorMessage = errorResponse.ErrorMessage;
                    Console.WriteLine($"[GetProductsWithOptionsAsync]-{response.StatusCode}-{apiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                apiResponse.ExceptionResult = ExceptionResult.InternalServerError500;
                apiResponse.ErrorMessage = ex.Message;
                Console.WriteLine($"[GetProductsWithOptionsAsync] {ex.Message}");
            }
            return apiResponse;
        }
        
        public async Task<bool> UpdateProductsRoleIdAsync(IEnumerable<UpdateProductRoleIdRequest> updateRequest)
        {
            bool apiResponse;
            try
            {
                var response = await _httpClient.PatchAsync("roleId", JsonContent.Create(updateRequest));
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
