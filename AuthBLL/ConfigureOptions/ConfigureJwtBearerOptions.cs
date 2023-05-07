using System.Text;
using AuthDAL.Settings;
using AuthService.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WritableConfig;
using AuthenticationSchemes = AuthDAL.Auth.AuthService.AuthenticationSchemes;

namespace AuthService.ConfigureNamedOptions;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JsonWebTokenAuthenticationSchemeOptions>
{
    private readonly IWritableConfig<AuthServiceSettings> _authServiceSettings;

    public ConfigureJwtBearerOptions(IWritableConfig<AuthServiceSettings> authServiceSettings)
    {
        _authServiceSettings = authServiceSettings;
    }

    public void Configure(string name, JsonWebTokenAuthenticationSchemeOptions options)
    {
        var authServiceSettingsConfig = _authServiceSettings.GetConfigObject();
        
        options.TokenValidationParameters = name switch
        {
            AuthenticationSchemes.JsonWebToken => new TokenValidationParameters
            {
                ValidateIssuer = authServiceSettingsConfig.JsonWebTokenValidateIssuer,
                ValidateAudience = authServiceSettingsConfig.JsonWebTokenValidateAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = authServiceSettingsConfig.JsonWebTokenValidateIssuerSigningKey,
                ValidIssuer = authServiceSettingsConfig.JsonWebTokenIssuer,
                ValidAudience = authServiceSettingsConfig.JsonWebTokenAudience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authServiceSettingsConfig.JsonWebTokenIssuerSigningKey))
            },
            _ => options.TokenValidationParameters
        };
    }

    public void Configure(JsonWebTokenAuthenticationSchemeOptions options)
    {
        Configure(string.Empty, options);
    }
}