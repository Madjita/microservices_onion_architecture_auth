using Logging.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.WithCaller;
using WritableConfig;
using ILogger = Serilog.ILogger;

namespace Logging.Factories;

public interface ICustomLoggerFactory
{
    public ILogger<T> Create<T>(string loggerAlias);
    public ILogger Create(string loggerAlias);
    public void UpdateConfig(string loggerAlias, Action<CustomLoggerConfigurationRoot> updateAction);
}

public class CustomLoggerFactory : ICustomLoggerFactory
{
    private readonly Dictionary<string, ILoggerFactory> _loggerFactories = new();
    private readonly Dictionary<string, IWritableConfig<CustomLoggerConfigurationRoot>> _loggerWritableConfigs = new ();
    private readonly string _configFilePath;
    private readonly IReadOnlyCollection<string> _loggerAliases;

    public CustomLoggerFactory(IReadOnlyCollection<string> loggerAliases, IServiceProvider serviceProvider, string configFilePath)
    {
        _loggerAliases = loggerAliases;
        _configFilePath = configFilePath;

        var writableConfigs = serviceProvider.GetServices<IWritableConfig<CustomLoggerConfigurationRoot>>().ToArray();
        foreach (var writableConfig in writableConfigs)
        {
            var loggerAlias = writableConfig.GetSection();
            writableConfig.AddHandlerToNotification(UpdateFactories);
            _loggerWritableConfigs.Add(loggerAlias, writableConfigs.FirstOrDefault(_ => _.GetSection() == loggerAlias));
        }
        
        UpdateFactories();
    }

    /// <summary>
    /// Creates a logger instance
    /// </summary>
    /// <param name="loggerAlias">Alias of a logger</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ILogger<T> Create<T>(string loggerAlias)
    {
        if (!_loggerFactories.ContainsKey(loggerAlias))
            throw new Exception($"Logger {nameof(loggerAlias)} not found!");

        return _loggerFactories[loggerAlias].CreateLogger<T>();
    }
    
    /// <summary>
    /// Creates a logger instance
    /// </summary>
    /// <param name="loggerAlias">Alias of a logger</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ILogger Create(string loggerAlias)
    {
        if (!File.Exists(_configFilePath))
            throw new FileNotFoundException("Config file not found!", _configFilePath);
        
        var configuration = new ConfigurationBuilder().AddJsonFile(_configFilePath, false, true).Build();
        
        if (!_loggerFactories.ContainsKey(loggerAlias))
            throw new Exception($"Logger {nameof(loggerAlias)} not found!");

        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, loggerAlias)
            .Enrich.WithCaller(true)
            .CreateLogger();
    }

    /// <summary>
    /// Updates factory configuration
    /// </summary>
    /// <param name="loggerAlias">Alias of a logger</param>
    /// <param name="updateAction">Update action</param>
    /// <exception cref="Exception"></exception>
    public void UpdateConfig(string loggerAlias, Action<CustomLoggerConfigurationRoot> updateAction)
    {
        if (!_loggerWritableConfigs.ContainsKey(loggerAlias))
            throw new Exception($"Logger {nameof(loggerAlias)} not found!");

        var writableConfig = _loggerWritableConfigs[loggerAlias];
        
        var config = writableConfig.GetConfigObject();
        updateAction(config);
        writableConfig.SetConfig(config);
    }
    
    private void UpdateFactories(object sender = null, EventArgs e = null)
    {
        if (!File.Exists(_configFilePath))
            throw new FileNotFoundException("Config file not found!", _configFilePath);
        
        _loggerFactories.Clear();
    
        var configuration = new ConfigurationBuilder().AddJsonFile(_configFilePath, false, true).Build();
        
        foreach (var loggerAlias in _loggerAliases)
        {
            var loggerConfigurationConsole = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration, loggerAlias)
                .Enrich.WithCaller(true);

            _loggerFactories.Add(loggerAlias, new LoggerFactory().AddSerilog(loggerConfigurationConsole.CreateLogger(), true));
        }
    }
}