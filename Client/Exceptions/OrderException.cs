namespace ProPayments.Client.Exceptions
{
    public class OrderException : Exception
    {
        public int StatusCodes { get; }

        public OrderException(int statusCodes, string message) : base(message)
        {
            StatusCodes = statusCodes;
        }
    }
}
