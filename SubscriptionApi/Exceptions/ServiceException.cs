using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Exceptions
{
    public class ServiceException : Exception
    {
        public int StatusCode { get; }
        public ServiceResult ServiceResult { get; }
        public ServiceException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
            ServiceResult = ServiceResult.Default;
        }
        public ServiceException(int statusCode, ServiceResult servicResult, string message) : base(message)
        {
            StatusCode = statusCode;
            ServiceResult = servicResult;
        }
    }
}
