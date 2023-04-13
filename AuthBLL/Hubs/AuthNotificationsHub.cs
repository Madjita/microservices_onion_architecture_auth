using System;
using System.Threading.Tasks;
using AuthBLL.Services.Advanced;
using AuthDAL.Models;
using AuthService.Services.Advanced;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MobileDrill_Common.Base;
using AuthenticationSchemes = AuthDAL.Auth.AuthService.AuthenticationSchemes;

namespace AuthBLL.Hubs;

public interface IAuthNotificationsHub : IHubBaseAction
{

}
[Authorize(AuthenticationSchemes = $"{AuthenticationSchemes.AccessToken},{AuthenticationSchemes.JsonWebToken}")]
public class AuthNotificationsHub : HubBase<IAuthNotificationsHub>
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IJsonWebTokenAdvancedService _jsonWebTokenAdvancedService;
    private readonly ILogger<HubBase<IAuthNotificationsHub>> _logger;
    private readonly IAccountAdvancedService _accountAdvancedService;

    public AuthNotificationsHub(
        IAccountAdvancedService accountAdvancedService,
        IJsonWebTokenAdvancedService jsonWebTokenAdvancedService,
        ILogger<HubBase<IAuthNotificationsHub>> logger,
        IHostEnvironment hostEnvironment
    ) : base(logger, hostEnvironment)
    {
        _accountAdvancedService = accountAdvancedService;
        _jsonWebTokenAdvancedService = jsonWebTokenAdvancedService;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    /*
     * This was left for an example of SignalR Hub Endpoint
     */
    // /// <summary>
    // ///     This endpoint is used to notify client about file processing status.
    // ///     It is triggered for every file
    // /// </summary>
    // /// <param name="message"></param>
    // [Authorize(nameof(AccessTokenSystemAuthorizationRequirement))]
    // public async Task FileProcessingResult(string message)
    // {
    //     _logger.LogInformation(Localize.Log.MethodStart(GetType(), nameof(FileProcessingResult)));
    //
    //     try
    //     {
    //         AbortDisconnectAfter();
    //
    //         var data = JsonSerializer.Deserialize<FileProcessingDto>(message);
    //
    //         if (ValidateModel(data) is { } validationResult)
    //             ThrowError(validationResult);
    //
    //         var file = await _fileEntityService.GetByIdLocallyAsync(data.FileId);
    //         if (file is not {UserId: { }})
    //             ThrowException(new CustomException(Localize.Error.FileNotFound));
    //
    //         if (file.UserId == Consts.PublicUserId)
    //             for (var page = 1;; page += 1)
    //             {
    //                 var userToUserGroupMappings = await _userToUserGroupMappingEntityService.GetByEntityRightIdAsync(Consts.RootUserGroupId, new PageModel
    //                 {
    //                     Page = page,
    //                     PageSize = 512
    //                 });
    //
    //                 foreach (var userToUserGroupMapping in userToUserGroupMappings.entities)
    //                     //TODO: Automapper
    //                     await Clients.Group(string.Format(Localize.Text.SignalRHubGroupKey, userToUserGroupMapping.EntityLeftId)).SendAsync("ReceiveFileProcessingResult",
    //                         new FileProcessingResultDto
    //                         {
    //                             FileId = data.FileId,
    //                             Status = data.Status,
    //                             Progress = data.Progress
    //                         });
    //
    //                 if (userToUserGroupMappings.entities.Count < 512)
    //                     break;
    //             }
    //         else
    //             //TODO: Automapper
    //             await Clients.Group(string.Format(Localize.Text.SignalRHubGroupKey, file.UserId)).SendAsync("ReceiveFileProcessingResult", new FileProcessingResultDto
    //             {
    //                 FileId = data.FileId,
    //                 Status = data.Status,
    //                 Progress = data.Progress
    //             });
    //
    //         _logger.LogInformation(Localize.Log.MethodEnd(GetType(), nameof(FileProcessingResult)));
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(Localize.Log.MethodError(GetType(), nameof(FileProcessingResult), e.Message));
    //
    //         ThrowException(e, ErrorType.Unhandled);
    //     }
    // }

    #region Overrides
    
    public override async Task OnConnectedAsync()
    {
        var userId = _accountAdvancedService.GetIdFromClaims(false);

        if (userId != default)
        {
            var key = string.Format(Localize.Text.SignalRHubGroupKey, userId);

            await Groups.AddToGroupAsync(Context.ConnectionId, key);
            AddDisconnectFilter(Context.ConnectionId, () => (_jsonWebTokenAdvancedService.GetExpiresAtFromClaims() < DateTimeOffset.UtcNow, Localize.Error.JsonWebTokenExpired.ToString()));
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var user = _accountAdvancedService.GetIdFromClaims(false);

        if (user != default)
        {
            var key = string.Format(Localize.Text.SignalRHubGroupKey, user);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, key);
        }

        DeleteDisconnectFilter(_ => _.Key.connectionId == Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    #endregion
}