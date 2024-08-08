namespace ProPayments.Service.Clients.Dtos.Solana.Response
{
    public class RpcSignaturesAddressResponse
    {
        public string Jsonrpc { get; set; }
        //public long Id { get; set; }
        public List<SolanaSignature> Result { get; set; }
    }

    public class SolanaSignature
    {
        public string Signature { get; set; }
    }
}
