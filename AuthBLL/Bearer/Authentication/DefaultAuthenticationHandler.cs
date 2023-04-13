using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthDAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthService.Authentication;

public class DefaultAuthenticationHandler : AuthenticationHandler<DefaultAuthenticationSchemeOptions>
{
    public DefaultAuthenticationHandler(
        IOptionsMonitor<DefaultAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // return Task.FromResult(AuthenticateResult.NoResult());

        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimKey.IsPublic.ToString(), true.ToString(), ClaimValueTypes.Boolean)
        }, nameof(DefaultAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}