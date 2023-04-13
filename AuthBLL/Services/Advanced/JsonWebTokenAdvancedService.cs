using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthBLL.Repository.Base;
using AuthDAL.Entities;
using AuthDAL.Enums;
using AuthDAL.Exceptions;
using AuthDAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MobileDrill.DataBase.Data;

namespace AuthBLL.Services.Advanced;

public interface IJsonWebTokenAdvancedService
{
    Task<JsonWebToken> GetFromHttpContext(bool throwIfNotProvided = true);
    DateTimeOffset GetExpiresAtFromClaims(bool throwIfNotProvided = true);

    string CreateWithClaims(
        string issuerSigningKey,
        string issuer,
        string audience,
        IEnumerable<Claim> claims,
        DateTime expires
    );
}

public class JsonWebTokenAdvancedService : IJsonWebTokenAdvancedService
{
    private readonly HttpContext _httpContext;
    private readonly IRepositoryBase<AuthDbContext,JsonWebToken> _tokenRepository;
    private readonly ILogger<JsonWebTokenAdvancedService> _logger;

    public JsonWebTokenAdvancedService(
        ILogger<JsonWebTokenAdvancedService> logger,
        IRepositoryBase<AuthDbContext,JsonWebToken> tokenRepository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
        _httpContext = httpContextAccessor.HttpContext;
    }


    public async Task<JsonWebToken> GetFromHttpContext(bool throwIfNotProvided = true)
    {
        if (!int.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.JsonWebTokenId.ToString())?.Value, out var jsonWebTokenId))
            return throwIfNotProvided
                ? throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth, Localize.Error.JsonWebTokenNotProvided.ToString())
                : null;

        var entity = await _tokenRepository.GetByIdAsync(jsonWebTokenId);
        return entity;
    }

    public DateTimeOffset GetExpiresAtFromClaims(bool throwIfNotProvided = true)
    {
        if (!int.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.ExpiresAt.ToString())?.Value, out var expiresAt))
            return throwIfNotProvided
                ? throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth, Localize.Error.JsonWebTokenNotProvided.ToString())
                : default;

        return DateTimeOffset.FromUnixTimeSeconds(expiresAt);
    }

    public string CreateWithClaims(
        string issuerSigningKey,
        string issuer,
        string audience,
        IEnumerable<Claim> claims,
        DateTime expires
    )
    {
        var symmetricSecurityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(issuer, audience,
            claims ?? new List<Claim>(), expires: expires, signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}