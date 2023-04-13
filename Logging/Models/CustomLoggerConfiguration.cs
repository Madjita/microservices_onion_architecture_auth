namespace Logging.Models;

/// <summary>
/// Маппинг структуры конфигов Serilog
/// </summary>

public class CustomLoggerConfigurationArgs
{
    public string Path { get; set; }
    public string OutputTemplate { get; set; }
    public string restrictedToMinimumLevel { get; set; }
}

public class CustomLoggerConfigurationRoot
{
    public List<string> Using { get; set; }
    public List<string> Enrich { get; set; }
    public List<CustomLoggerConfigurationWriteTo> WriteTo { get; set; }
    public string MinimumLevel { get; set; }
}

public class CustomLoggerConfigurationWriteTo
{
    public string Name { get; set; }
    public CustomLoggerConfigurationArgs Args { get; set; }
}