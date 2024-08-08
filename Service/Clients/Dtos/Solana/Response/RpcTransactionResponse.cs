namespace ProPayments.Service.Clients.Dtos.Solana.Response
{
    public class RpcTransactionResponse
    {
        public string? Jsonrpc { get; set; }
        public Result? Result { get; set; }
        public long? Id { get; set; }
    }

    public class AccountKey
    {
        public string? Pubkey { get; set; }
        public bool? Signer { get; set; }
        public string? Source { get; set; }
        public bool? Writable { get; set; }
    }

    public class Info
    {
        public string Destination { get; set; } = string.Empty;
        public decimal Lamports { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class Instruction
    {
        public List<object>? Accounts { get; set; }
        public string? Data { get; set; }
        public string? ProgramId { get; set; }
        public object? StackHeight { get; set; }
        public Parsed? Parsed { get; set; }
        public string? Program { get; set; }
    }

    public class Message
    {
        public List<AccountKey>? AccountKeys { get; set; }
        public List<Instruction>? Instructions { get; set; }
        public string? RecentBlockhash { get; set; }
    }

    public class Meta
    {
        public long? ComputeUnitsConsumed { get; set; }
        public object? Err { get; set; }
        public long? Fee { get; set; }
        public List<object>? InnerInstructions { get; set; }
        public List<string>? LogMessages { get; set; }
        public List<long>? PostBalances { get; set; }
        public List<object>? PostTokenBalances { get; set; }
        public List<long>? PreBalances { get; set; }
        public List<object>? PreTokenBalances { get; set; }
        public List<object>? Rewards { get; set; }
        public Status? Status { get; set; }
    }

    public class Parsed
    {
        public Info? Info { get; set; }
        public string? Type { get; set; }
    }

    public class Result
    {
        public long? BlockTime { get; set; }
        public Meta? Meta { get; set; }
        public long? Slot { get; set; }
        public BlockchainTransaction? Transaction { get; set; }
    }

    public class Status
    {
        public object? Ok { get; set; }
    }

    public class BlockchainTransaction
    {
        public Message? Message { get; set; }
        public List<string>? Signatures { get; set; }
    }
}
