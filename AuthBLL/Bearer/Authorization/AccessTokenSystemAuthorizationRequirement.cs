using Microsoft.AspNetCore.Authorization;

namespace AuthService.Authorization;

public class AccessTokenSystemAuthorizationRequirement : IAuthorizationRequirement
{
    public AccessTokenSystemAuthorizationRequirement(string accessToken)
    {
        AccessToken = accessToken;
    }

    public string AccessToken { get; }
}