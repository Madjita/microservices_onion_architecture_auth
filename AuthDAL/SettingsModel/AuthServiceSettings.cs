namespace AuthDAL.Settings;

public class AuthServiceSettings
{
    public string SignalRSystemAccessToken { get; set; }
    public string JsonWebTokenIssuer { get; set; }
    public bool JsonWebTokenValidateIssuer { get; set; }
    public string JsonWebTokenAudience { get; set; }
    public bool JsonWebTokenValidateAudience { get; set; }
    public string JsonWebTokenIssuerSigningKey { get; set; }
    public bool JsonWebTokenValidateIssuerSigningKey { get; set; }
    public int JsonWebTokenExpirySeconds { get; set; }
}