using Probot.SubscriptionApi.Clients.Dtos;
using Probot.SubscriptionApi.Clients.Dtos.Solana.Response;

namespace Probot.SubscriptionApi.Clients.IClients;
public interface ISolanaRpcClient
{
    Task<long> GetBalanceAsync(string walletAddress);
    Task<Result?> GetTransactionDetailsAsync(string signature);
    Task<ClientResponse<List<string>>> GetTransactionsHashAsync(string accountPubkey, int? numberOfTransactions = null, string? before = null, string? until = null);
}