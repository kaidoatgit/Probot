using ProPayments.Service.Clients.Dtos;
using ProPayments.Service.Clients.Dtos.Solana.Response;

namespace ProPayments.Service.Clients.IClients
{
    public interface ISolanaRpcClient
    {
        Task<long> GetBalanceAsync(string walletAddress);
        Task<Result?> GetTransactionDetailsAsync(string signature);
        Task<ClientResponse<List<string>>> GetTransactionsHashAsync(string accountPubkey, int? numberOfTransactions = null, string? before = null, string? until = null);
    }
}