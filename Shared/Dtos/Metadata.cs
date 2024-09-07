using Probot.Shared.Enums;

namespace Probot.Shared.Dtos;

public class Metadata
{
    public int StatusCode { get; set; }
    public ServiceResult ServiceResult { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
