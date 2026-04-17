using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ProductService.IntegrationTests.Config;

public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string AuthenticationScheme = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var key = "Authorization";

        if (!Request.Headers.ContainsKey(key))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authorizationValue = Request.Headers["Authorization"][0];

        if (string.IsNullOrEmpty(authorizationValue))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (authorizationValue.Split(' ').Length != 2)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authorizationValue.Split(' ')[1];
        var hanlder = new JwtSecurityTokenHandler();
        var jwt = hanlder.ReadJwtToken(token);

        var identity = new ClaimsIdentity(jwt.Claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
