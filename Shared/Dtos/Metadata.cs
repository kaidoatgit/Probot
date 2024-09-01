namespace Probot.Shared.Dtos;

public class Metadata
{
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;
}
