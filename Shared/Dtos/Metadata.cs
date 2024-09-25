using Probot.Shared.Enums;

namespace Probot.Shared.Dtos;

public class ErrorResponse
{
    public ExceptionResult ExceptionResult { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
