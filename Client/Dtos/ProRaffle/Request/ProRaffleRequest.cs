namespace ProPayments.Client.Dtos.ProRaffle.Request
{
    public class ProRaffleRequest
    {
        public ulong UserId { get; set; }
        public string Code { get; internal set; } = null!;
        public string AlphabotKey { get; internal set; } = null!;
    }
}
