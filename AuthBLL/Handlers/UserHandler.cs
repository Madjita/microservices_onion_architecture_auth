using AuthBLL.Base;
using AuthDAL.Dtos;
using AuthDAL.Enums;
using AuthDAL.Exceptions;
using AuthDAL.Models;
using AuthService.Services.Advanced;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MobileDrill.DataBase.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AuthBLL.Handlers;

public interface IUserHandler
{
    Task<IDtoResultBase> Read(CancellationToken cancellationToken = default);
}

public class UserHandler : HandlerBase, IUserHandler
{
    private readonly ILogger<HandlerBase> _logger;
    private readonly IAccountAdvancedService _accountAdvancedService;

    public UserHandler(
        ILogger<HandlerBase> logger,
        IAccountAdvancedService accountAdvancedService
    )
    {
        _logger = logger;
        _accountAdvancedService = accountAdvancedService;
    }

    public async Task<IDtoResultBase> Read(CancellationToken cancellationToken = default)
    {
        var user = await _accountAdvancedService.GetFromHttpContext() ?? throw new HttpResponseException(StatusCodes.Status400BadRequest,
            ErrorType.HttpContext, Localize.Error.UserNotFoundOrHttpContextMissingClaims.ToString());
    
        return new UserReadResultDto
        {
            Name = user.Email,
            Surname = user.Surname,
            Patronymic = user.Patronymic,
            Email = user.Email
        };
    }
}