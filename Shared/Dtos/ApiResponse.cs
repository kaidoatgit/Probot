
using Probot.Shared.Enums;

namespace Probot.Shared.Dtos;
public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public ServiceResult ServiceResult { get; set; }
    public T? Data { get; set; }
}
