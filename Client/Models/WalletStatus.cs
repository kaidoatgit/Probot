namespace ProPayments.Client.Models
{
    public class WalletStatus
    {
        public Result Result { get; set; } = Result.Default;
        public string Message { get; set; } = string.Empty;
    }

    public enum Result
    {
        Default,
        WalletExist,
        WalletFoundInOrder
    }
}
