using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthBLL.Services.Repository;
using AuthDAL.Auth.AuthService;
using AuthDAL.Entities;
using AuthDAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AuthService.Authentication;

public class JsonWebTokenAuthenticationHandler : AuthenticationHandler<JsonWebTokenAuthenticationSchemeOptions>
{
    private readonly JsonWebTokenAuthenticationSchemeOptions _jsonWebTokenAuthenticationSchemeOptions;
    private readonly IRepository<JsonWebToken> _jsonWebTokenRepository;

    public JsonWebTokenAuthenticationHandler(
        IOptionsMonitor<JsonWebTokenAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IRepository<JsonWebToken> jsonWebTokenRepository
    ) : base(options, logger, encoder, clock)
    {
        _jsonWebTokenAuthenticationSchemeOptions = options.Get(AuthenticationSchemes.JsonWebToken);
        _jsonWebTokenRepository = jsonWebTokenRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var executingEndpoint = Context.GetEndpoint();

        if (executingEndpoint == null)
            return AuthenticateResult.Fail(new NullReferenceException(nameof(executingEndpoint)));

        if (executingEndpoint.Metadata.OfType<AllowAnonymousAttribute>().Any())
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));

        var authorizationBearerPayloads = new[]
        {
            Context.Request.Headers[HeaderNames.Authorization].SingleOrDefault()?.Split(" ").Last(),
            Context.Request.Query.SingleOrDefault(_ => _.Key == QueryKey.JsonWebToken.ToString()).Value.ToString()
        };

        authorizationBearerPayloads = authorizationBearerPayloads.Where(_ => !string.IsNullOrEmpty(_)).ToArray();

        if (authorizationBearerPayloads.Length == 0)
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenNotProvided.ToString());

        var claims = new List<Claim>();

        string authorizationBearerPayload = null;

        foreach (var authorizationBearerPayloadTemp in authorizationBearerPayloads)
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(authorizationBearerPayloadTemp,
                    _jsonWebTokenAuthenticationSchemeOptions.TokenValidationParameters,
                    out var validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;

                claims.AddRange(jwtToken.Claims);

                authorizationBearerPayload = authorizationBearerPayloadTemp;
                break;
            }
            catch (Exception)
            {
                // ignored
            }

        if (string.IsNullOrEmpty(authorizationBearerPayload))
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenIdRetrievalFailed.ToString());

        var jsonWebToken = await _jsonWebTokenRepository.SingleOrDefaultAsync(_ => _.Token == authorizationBearerPayload);
        if (jsonWebToken == null)
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenNotFound.ToString());
        
        if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenExpired.ToString());

        var claimsIdentity = new ClaimsIdentity(claims, nameof(JsonWebTokenAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}