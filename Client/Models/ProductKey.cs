namespace Probot.Client.Models;

public class ProductKey
{
    public string Code { get; set; } = string.Empty;
    public ProductOption ProductOption { get; set; } = null!;
}