using AuthService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using AuthenticationSchemes = AuthDAL.Auth.AuthService.AuthenticationSchemes;

namespace AuthServiceV2.ConfigureOptions;

public class ConfigureAuthenticationOptions : IConfigureOptions<AuthenticationOptions>
{
    public void Configure(AuthenticationOptions options)
    {
        options.DefaultAuthenticateScheme = AuthenticationSchemes.Default;
        options.DefaultChallengeScheme = null;
    }
}