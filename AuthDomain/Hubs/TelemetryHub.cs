#nullable enable

using System;
using System.Threading.Tasks;
using Logging.Factories;
using AuthDomain.Helpers;
using Microsoft.AspNetCore.SignalR;
using ILogger = Serilog.ILogger;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using AuthDAL.Auth.AuthService;
using System.Text.Json;

namespace AuthDomain.Hubs.AppAuth;

public class GetGpsCoordinates
{
    [JsonPropertyName("longitude")]
    public double Longitude {get;set;}
    [JsonPropertyName("latitude")]
    public double Latitude {get;set;}
    [JsonPropertyName("timestamp")]
    public double Timestamp {get;set;} //millisecondsSinceEpoch
    [JsonPropertyName("accuracy")]
    public double Accuracy {get;set;}
    [JsonPropertyName("altitude")]
    public double Altitude {get;set;}
    [JsonPropertyName("floor")]
    public double? Floor {get;set;}
    [JsonPropertyName("heading")]
    public double Heading {get;set;}
    [JsonPropertyName("speed")]
    public double Speed {get;set;}
    [JsonPropertyName("speed_accuracy")]
    public double SpeedAccuracy {get;set;}
    [JsonPropertyName("is_mocked")]
    public bool IsMocked {get;set;}

}
public interface ITelemetryHubClient
{

}

// [Authorize(AuthenticationSchemes = $"{AuthenticationSchemes.AccessToken},{AuthenticationSchemes.JsonWebToken}")]
public class TelemetryHub : Hub<ITelemetryHubClient>, IDisposable
{
    private readonly ILogger _logger;

    public TelemetryHub(ICustomLoggerFactory customLoggerFactory)
    {
        _logger = customLoggerFactory.Create("MD_Main");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        _logger.Information("TelemetryHub hub connect");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {    
       try
       {
            await base.OnDisconnectedAsync(exception);
            _logger.Information("TelemetryHub hub disconnect");
       }
       catch (System.Exception e)
       {
            _logger.Error($"TelemetryHub hub disconnect {e.Message}");
       }
    }

    public async Task GetGpsCoordinatesAsync(string jsonString)
    {
        _logger.Information($"TelemetryHub ${jsonString}");
        var obj = JsonSerializer.Deserialize<GetGpsCoordinates>(jsonString);
        _logger.Information($"TelemetryHub Serialize : {JsonSerializer.Serialize(obj)}");
    }
}
