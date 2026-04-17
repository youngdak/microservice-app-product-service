using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace ProductService.IntegrationTests.Config;

public static class MockJwtTokens
{
    public static string Issuer { get; } = "http://localhost:9090";
    public static string Audience { get; } = "hotelsync";
    public static SecurityKey SecurityKey { get; }
    public static SigningCredentials SigningCredentials { get; }

    private static readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

    static MockJwtTokens()
    {
        var secretKey = new byte[32];
        var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(secretKey);

        SecurityKey = new SymmetricSecurityKey(secretKey) { KeyId = Guid.NewGuid().ToString() };
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public static string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var allClaims = new List<Claim>(claims)
        {
            new("scope", "openid"),
            new("scope", "profile"),
            new("client_id", "dakolo-microservice")
        };
        return _tokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, allClaims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }
}
