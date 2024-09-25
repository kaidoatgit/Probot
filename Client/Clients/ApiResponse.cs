
using Probot.Shared.Enums;

namespace Probot.Client.Clients;
public class ApiResponse<T>
{
    public string? ErrorMessage { get; set; }
    public ExceptionResult ExceptionResult { get; set; }
    public T? Data { get; set; }
}
