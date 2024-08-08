namespace ProPayments.Client.Exceptions
{
    public class SubscriptionException : Exception
    {
        public int StatusCodes { get; }

        public SubscriptionException(int statusCodes, string message) : base(message)
        {
            StatusCodes = statusCodes;
        }
    }
}
