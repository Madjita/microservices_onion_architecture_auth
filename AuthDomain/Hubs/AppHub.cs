#nullable enable

using System;
using System.Threading.Tasks;
using Logging.Factories;
using AuthDomain.Helpers;
using Microsoft.AspNetCore.SignalR;
using ILogger = Serilog.ILogger;

namespace AuthDomain.Hubs.AppAuth;

public interface IAppHubClient
{
    public Task AppVersion(string version);
    public Task LoggingOffAsync(string token);
}


public class AppHub : Hub<IAppHubClient>, IDisposable
{
    private readonly ILogger _logger;

    public AppHub(ICustomLoggerFactory customLoggerFactory)
    {
        _logger = customLoggerFactory.Create("MD_Main");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.ConnectionId);
        _logger.Information("App hub connect");
        //Данный метод отправляет строку версии в 'AppVersion' Event...Спрашивать у фронтендера =) 
        await Clients.Group(Context.ConnectionId).AppVersion(AppVersion.Version);
        _logger.Information($"App hub send version: {AppVersion.Version}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {    
       try
       {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
            _logger.Information("App hub disconnect");
       }
       catch (System.Exception e)
       {
            _logger.Error($"App hub disconnect {e.Message}");
       }
    }

    public async Task LoggingOffAsync(string token)
    {
        await Clients.All.LoggingOffAsync(token);
    }
}
