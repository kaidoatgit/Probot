using System.Text.Json;
using System.Text;
using Probot.SubscriptionApi.Clients.IClients;
using Probot.SubscriptionApi.Clients.Dtos.Solana.Response;
using Probot.SubscriptionApi.Clients.Dtos;

namespace Probot.SubscriptionApi.Clients
{
    public class SolanaRpcClient : ISolanaRpcClient
    {
        private readonly HttpClient _httpClient;

        public SolanaRpcClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<long> GetBalanceAsync(string walletAddress)
        {
            var requestContent = new
            {
                jsonrpc = "2.0",
                id = 1,
                method = "getBalance",
                @params = new[] { walletAddress }
            };

            var httpContent = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(string.Empty, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var balanceResponse = JsonSerializer.Deserialize<RpcBalanceResponse>(responseContent, options);
            if (balanceResponse != null && balanceResponse.Result != null)
            {
                return (long)balanceResponse.Result.Value; // Convert lamports to SOL
            }

            throw new Exception("Failed to get balance from Solana RPC.");
        }

        public async Task<ClientResponse<List<string>>> GetTransactionsHashAsync(string accountPubkey, int? numberOfTransactions = null, string? before = null, string? until = null)
        {
            var options = new
            {
                encoding = "jsonParsed",
                limit = numberOfTransactions,
                commitment = "finalized",
                before,
                until
            };

            var requestContent = new
            {
                jsonrpc = "2.0",
                id = 1,
                method = "getSignaturesForAddress",
                @params = new object[] { accountPubkey, options }
            };

            ClientResponse<List<string>> clientResponse = new();
            try
            {
                var httpContent = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Empty, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var signaturesResponse = JsonSerializer.Deserialize<RpcSignaturesAddressResponse>(responseContent, jsonOptions);
                    clientResponse.Data = signaturesResponse?.Result?.Select(sig => sig.Signature).ToList();
                }
                else
                {
                    clientResponse.StatusCode = (int)response.StatusCode;
                    clientResponse.ErrorMessage = response.ReasonPhrase ?? string.Empty;
                    Console.WriteLine($"[GetTransactionsHashAsync] - StatusCode:{(int)response.StatusCode} {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                clientResponse.StatusCode = StatusCodes.Status500InternalServerError;
                clientResponse.ErrorMessage = ex.Message;
                Console.WriteLine($"[Exception][GetTransactionsHashAsync]: {ex.Message}");
            }
            return clientResponse;
        }

        public async Task<Result?> GetTransactionDetailsAsync(string signature)
        {
            var requestContent = new
            {
                jsonrpc = "2.0",
                id = 1,
                method = "getTransaction",
                @params = new object[] { signature, new { encoding = "jsonParsed", commitment = "finalized" } }
            };

            try
            {
                var httpContent = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(string.Empty, httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Console.WriteLine($"[GetTransactionDetailsAsync] response: {responseContent}");
                var transactionResponse = JsonSerializer.Deserialize<RpcTransactionResponse>(responseContent, options);
                return transactionResponse?.Result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][GetTransactionDetailsAsync]: {ex.Message}");
            }
            return null;
        }
    }
}
