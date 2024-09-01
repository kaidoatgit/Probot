namespace Probot.Shared.Dtos.User.Response;

public class UserMetricsResponse
{
    public ulong Id { get; set; }
    public string Username { get; set; }
    public string WalletAddress { get; set; }
    public string? Email { get; set; }

    public Dictionary<ulong, int> InactiveKeysPerProduct { get; set; } = new();
    public Dictionary<ulong, int> ActiveSubsPerProduct { get; set; } = new();
}
