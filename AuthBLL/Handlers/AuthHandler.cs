using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthBLL.Base;
using AuthBLL.Hubs;
using AuthBLL.Repository.Base;
using AuthBLL.Services;
using AuthBLL.Services.Advanced;
using AuthDAL.Dto.Generic;
using AuthDAL.Dtos;
using AuthDAL.Entities;
using AuthDAL.Enums;
using AuthDAL.Exceptions;
using AuthDAL.Models;
using AuthDAL.response_models;
using AuthDAL.Settings;
using AuthService.Services.Advanced;
using Logging.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MobileDrill.DataBase.Data;
using WritableConfig;
using ILogger = Serilog.ILogger;

namespace AuthBLL.Handlers;

public interface IAuthHandler
{
    Task<(IDtoResultBase, Account)> SignIn(Account data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> SignOut(CancellationToken cancellationToken = default);

}

public class AuthHandler : HandlerBase, IAuthHandler
{
    private readonly IAppDbContextTransactionAction<AuthDbContext> _appDbContextTransactionAction;
    private readonly IHubContext<AuthNotificationsHub> _authNotificationsHub;
    private readonly IJsonWebTokenAdvancedService _jsonWebTokenAdvancedService;
    private readonly IRepositoryBase<AuthDbContext,JsonWebToken> _jsonWebTokenRepository;
    private readonly IWritableConfig<AuthServiceSettings> _authServiceSettings;
    private readonly ILogger _logger;
    private readonly IAccountAdvancedService _accountAdvancedService;
    private readonly IRepositoryBase<AuthDbContext,Account> _accountRepository;
    private readonly IWarningService _warningService;
    private readonly ICustomLoggerFactory _customLoggerFactory;

    public AuthHandler(
        IHubContext<AuthNotificationsHub> authNotificationsHub,
        IJsonWebTokenAdvancedService jsonWebTokenAdvancedService,
        IRepositoryBase<AuthDbContext,JsonWebToken> jsonWebTokenRepository,
        IRepositoryBase<AuthDbContext,Account> accountRepository,
        IWritableConfig<AuthServiceSettings> authServiceSettings,
        IAccountAdvancedService accountAdvancedService,
        IWarningService warningService,
        IAppDbContextTransactionAction<AuthDbContext> appDbContextTransactionAction,
        ICustomLoggerFactory customLoggerFactory
    )
    {
        _logger = customLoggerFactory.Create("Auth");
        _jsonWebTokenRepository = jsonWebTokenRepository;
        _accountRepository = accountRepository;
        _accountAdvancedService = accountAdvancedService;
        _jsonWebTokenAdvancedService = jsonWebTokenAdvancedService;
        _warningService = warningService;
        _authNotificationsHub = authNotificationsHub;
        _customLoggerFactory = customLoggerFactory;
        _authServiceSettings = authServiceSettings;
        _appDbContextTransactionAction = appDbContextTransactionAction;
    }

    public async Task<(IDtoResultBase, Account)> SignIn(
        Account user,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _appDbContextTransactionAction.BeginTransactionAsync(cancellationToken);

            var dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;

            var authServiceSettingsConfig = _authServiceSettings.GetConfigObject();

            var jsonWebTokenExpiresAt = dateTimeOffsetUtcNow.AddSeconds(authServiceSettingsConfig.JsonWebTokenExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenAdvancedService.CreateWithClaims(authServiceSettingsConfig.JsonWebTokenIssuerSigningKey,
                authServiceSettingsConfig.JsonWebTokenIssuer, authServiceSettingsConfig.JsonWebTokenAudience, new List<Claim>
                {
                    new(ClaimKey.UserId.ToString(), user.Id.ToString(), ClaimValueTypes.String),
                    new(ClaimKey.RandomToken.ToString(), Guid.NewGuid().ToString(), ClaimValueTypes.String)
                }, jsonWebTokenExpiresAt.UtcDateTime);

                _jsonWebTokenRepository.SaveAndCommit(new JsonWebToken
                {
                    Token = jsonWebTokenString,
                    ExpiresAt = jsonWebTokenExpiresAt,
                    DeleteAfter = jsonWebTokenExpiresAt,
                    AccountId = user.Id
                });

            _warningService.Add(new WarningModelResultEntry(WarningType.Auth, Localize.Warning.XssVulnerable.ToString()));

            user.LastSignIn = dateTimeOffsetUtcNow.UtcDateTime;

            _accountRepository.SaveAndCommit(user);

            if (cancellationToken.IsCancellationRequested)
                throw new HttpResponseException(StatusCodes.Status408RequestTimeout, ErrorType.Generic, Localize.Error.RequestAbortedOrCancelled.ToString());

            await _appDbContextTransactionAction.CommitTransactionAsync(CancellationToken.None);


            return (new AuthSignInResultDto
            {
                Email = user.Email,
                JsonWebToken = jsonWebTokenString,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt
            }, user);
        }
        catch (Exception e)
        {
            await _appDbContextTransactionAction.RollbackTransactionAsync(CancellationToken.None);

            throw;
        }
    }

    public async Task<IDtoResultBase> SignOut(CancellationToken cancellationToken = default)
    {

        try
        {
            await _appDbContextTransactionAction.BeginTransactionAsync(cancellationToken);

            var user = await _accountAdvancedService.GetFromHttpContext();
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext, Localize.Error.UserNotFoundOrHttpContextMissingClaims.ToString());

            await _jsonWebTokenRepository.PurgeAsync(_ => _.AccountId == user.Id, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                throw new HttpResponseException(StatusCodes.Status408RequestTimeout, ErrorType.Generic, Localize.Error.RequestAbortedOrCancelled.ToString());

            await _appDbContextTransactionAction.CommitTransactionAsync(CancellationToken.None);

            await _authNotificationsHub.Clients.Group(string.Format(Localize.Text.SignalRHubGroupKey, user.Id)).SendAsync("ReceiveAuthSignOutResult", CancellationToken.None);

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextTransactionAction.RollbackTransactionAsync(CancellationToken.None);

            throw;
        }
    }
}