using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Exceptions;
public class SubscriptionException : Exception
{
    public ExceptionResult ExceptionResult { get; }
    public SubscriptionException(ExceptionResult exceptionResult, string message) : base(message)
    {
        ExceptionResult = exceptionResult;
    }
}

