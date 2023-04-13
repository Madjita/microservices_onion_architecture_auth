using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthBLL.Repository.Base;
using AuthDAL.Entities;
using AuthDAL.Enums;
using AuthDAL.Exceptions;
using AuthDAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MobileDrill.DataBase.Data;

namespace AuthService.Services.Advanced;

public interface IAccountAdvancedService
{
    Task<Account> GetFromHttpContext(bool throwIfNotProvided = true);
    int GetIdFromClaims(bool throwIfNotProvided = true, CancellationToken cancellationToken = default);
}

public class AccountAdvancedService : IAccountAdvancedService
{
    private readonly HttpContext _httpContext;
    private readonly ILogger<AccountAdvancedService> _logger;
    private readonly IRepositoryBase<AuthDbContext,Account> _accountRepository;

    public AccountAdvancedService(
        ILogger<AccountAdvancedService> logger,
        IRepositoryBase<AuthDbContext,Account> accountRepository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<Account> GetFromHttpContext(bool throwIfNotProvided = true)
    {
        if (!int.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.UserId.ToString())?.Value, out var userId) && throwIfNotProvided)
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth, Localize.Error.JsonWebTokenNotProvided.ToString());

        //Fallback to public user
        // userId = userId == default ? Consts.PublicUserId : userId;

        var entity = await _accountRepository.GetByIdAsync(userId);

        return entity;
    }

    public int GetIdFromClaims(bool throwIfNotProvided = true, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.UserId.ToString())?.Value, out var userId))
            return throwIfNotProvided
                ? throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth, Localize.Error.JsonWebTokenNotProvided.ToString())
                : default;

        //Fallback to public user
        // userId = userId == default ? Consts.PublicUserId : userId;

        return userId;
    }
}