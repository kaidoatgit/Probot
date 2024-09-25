using System.Security.Cryptography;
using System.Text;
using IdentityModel;
using Microsoft.Extensions.Options;
using Probot.ProRaffleTool.Options;
using Probot.ProRaffleTool.Services.Services.Abstractions;

namespace Probot.ProRaffleTool.Services.Services;

internal class TokenService : ITokenService
{
    private readonly OAuthSettings _oauthSettings;
    public TokenService(IOptions<OAuthSettings> oauthOptions)
    {
        _oauthSettings = oauthOptions.Value;
    }
    
    public (string token, string hashedToken) CreateTokens(string? token)
    {
        token ??= CryptoRandom.CreateUniqueId(32, CryptoRandom.OutputFormat.Hex);
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_oauthSettings.StateKey));
        byte[] hashedTokenBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        var hashedToken = BitConverter.ToString(hashedTokenBytes).Replace("-", "").ToLower();

        return (token, hashedToken);
    }
}
