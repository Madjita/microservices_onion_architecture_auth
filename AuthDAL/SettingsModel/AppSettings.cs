using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AuthDAL.Settings;

public class AppSettings
{
    public Logging Logging { get; set; } = new Logging();
    public string EnableMuwTelemetryToCan { get; set; } = "True";
    public Unit Unit { get; set; } = new Unit();
    public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
    public CoreService CoreService { get; set; } = new CoreService();
    public Hpp Hpp { get; set; } = new Hpp();
    public DIPUSettings DIPUSettings { get; set; } = new DIPUSettings();
    public VDTranslation VDTranslation { get; set; } = new VDTranslation();
    
    
    public List<string> SettingsPaths { get; set; } = new List<string>
    {
        "algorithmssettings.json",
        "calibrationsettings.json",
        "candevices.json",
        "cansettings.json",
        "dasdevices.json",
        "drillsettings.json",
        "drillnorms.json",
        "inclinometers.json",
        "jacksettings.json",
        "inputs.json",
        "ssalgorithms.json",
        "telemetrysettings.json",
        "telemetrytocan.json",
        "unitsettings.json",
        "dassettings.json",
        "dascalibrationsettings.json",
        "logSettings.json",
        "jacksLimitSwitcheTestSettings.json",
        "statussettings.json",
        "autoselecthole_settings.json",
        "authServiceSettings.json",
        "can_setup.json",
        "SerilogSettings.json"
    };
}
public class LogLevel
{
    public string Default  { get; set; } = "Information";
    [JsonProperty("Microsoft.AspNetCore")]
    public string Microsoft_AspNetCore { get; set; } = "Information";
}
public class Logging
{
    public LogLevel LogLevel { get; set; } = new LogLevel();
}
public class Unit
{
    public int AreaId { get; set; } = 1;
}
public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = "Data Source=RIT.db";
}
public class CoreService
{
    public string Https { get; set; } = "https://10.0.2.2:5001/";
    public string Http { get; set; } = "http://10.0.2.2:5000/";
    public string MMS { get; set; } = "http://172.19.10.60:8014/";
}
public class Hpp
{
    public string HppSerialPort { get; set; } = "/dev/ttyS0";
    public string HppVirtualPort { get; set; } = "/dev/ttyS0";
    public int HppSerialPortBaudRate { get; set; } = 115200;
    public string HppLogEnabled { get; set; } = "true";
}
public class DIPUSettings
{
    public int SamplingRate { get; set; } = 4;
}
public class Elements
{
    public string Pgn { get; set; }
    public string Action { get; set; }
}

public class Serilog
{
    public Bootstrap Bootstrap { get; set; }
    public Default Default { get; set; }
}
public class VDTranslation
{
    public List<Elements> Elements { get; set; } = new List<Elements>
    {
        new Elements(){
            Pgn = "0x004601",
            Action = "1"
        },
        new Elements(){
            Pgn = "0x004602",
            Action = "2"
        },
        new Elements(){
            Pgn = "0x004603",
            Action = "4"
        }
    };
}

public class Bootstrap
{
    public List<string> Using { get; set; }
    public List<string> Enrich { get; set; }
    public MinimumLevel MinimumLevel { get; set; }
    public List<WriteTo> WriteTo { get; set; }
}
public class Default
{
    public List<string> Using { get; set; }
    public List<string> Enrich { get; set; }
    public MinimumLevel MinimumLevel { get; set; }
    public List<WriteTo> WriteTo { get; set; }
}
public class MinimumLevel
{
    public string Default { get; set; }
    public Override Override { get; set; }
}
public class WriteTo
{
    public string Name { get; set; }
    public Args Args { get; set; }
}
public class Override
{
    public string Microsoft { get; set; }
}
public class Args
{
    public string outputTemplate { get; set; }
}
