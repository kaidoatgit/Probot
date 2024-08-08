namespace ProPayments.Service.Clients.Dtos.Solana.Response
{
    public class RpcBalanceResponse
    {
        public string Jsonrpc { get; set; }
        public int Id { get; set; }
        public SolanaBalanceResult Result { get; set; }
    }

    public class SolanaBalanceResult
    {
        public SolanaContext Context { get; set; }
        public ulong Value { get; set; }
    }

    public class SolanaContext
    {
        public string ApiVersion { get; set; }
        public ulong Slot { get; set; }
    }
}
