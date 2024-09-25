
namespace Probot.ProRaffleTool.Services.Services.Abstractions;

public interface ITokenService
{
    (string token, string hashedToken) CreateTokens(string? state = null);
}
