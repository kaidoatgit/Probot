namespace Probot.Shared.Dtos.User.Response;
public class UserResponse
{
    public ulong Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; }
    public string WalletAddress { get; set; }
    public string? Email { get; set; }
}
