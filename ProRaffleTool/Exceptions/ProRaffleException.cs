using Probot.Shared.Enums;

namespace Probot.ProRaffleTool.Exceptions;

public class ProRaffleException : Exception
{
    public ExceptionResult ExceptionResult { get; }
    public ProRaffleException(ExceptionResult exceptionResult, string message) : base(message)
    {
        ExceptionResult = exceptionResult;
    }
}
