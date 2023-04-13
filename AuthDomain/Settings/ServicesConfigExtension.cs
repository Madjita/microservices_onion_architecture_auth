
using AuthDAL.Settings;
using Logging.Factories;
using Logging.Models;
using Microsoft.Extensions.Options;
using AuthDomain.Settings;
using WritableConfig;

namespace AuthDomain.Settings
{
    public static class ServicesConfigExtension
    {
        private static ISettingPathConfig _settingsPath;

        public static void AddConfigServices(this IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            _settingsPath = new SettingPathConfig();

            AddJsonFiles(_settingsPath, (ConfigurationManager)configuration);
            AddServices(_settingsPath, services, configuration);
        }

        private static void AddJsonFileAppsettings(ISettingPathConfig settingsPath, ConfigurationManager config)
        {
            string fileName = "appsettings.json";

            if (settingsPath.CheckSettingFile(fileName))
            {
                string filePath = settingsPath.GetSettingsPath() + fileName;
                config.AddJsonFile(filePath);
            }
        }

        private static void AddJsonFiles(ISettingPathConfig settingsPath, ConfigurationManager config)
        {
            AddJsonFileAppsettings(settingsPath, config);
            AddJsonFilesFromSettings(settingsPath, config);
        }

        private static void AddJsonFilesFromSettings(ISettingPathConfig settingsPath, ConfigurationManager config)
        {
            List<string> _inputSettings = config.GetSection("SettingsPaths").Get<List<string>>();

            foreach (var fileName in _inputSettings)
            {
                if (settingsPath.CheckSettingFile(fileName))
                {
                    string filePath = settingsPath.GetSettingsPath() + fileName;
                    config.AddJsonFile(filePath);
                }
            }
        }

        private static void AddServices(ISettingPathConfig settingsPath, IServiceCollection services, IConfiguration Configuration)
        {
            AddServicesAppsettings(settingsPath, services, Configuration);
            AddServicesForAlgorithms(settingsPath, services, Configuration);
        }

        private static void AddServicesForAlgorithms(ISettingPathConfig settingsPath, IServiceCollection services, IConfiguration Configuration)
        {
            string json_settings_path = settingsPath.GetSettingsPath();
            // services.ConfigureWritableConfig<CalibrationSettings>(Configuration.GetSection("CalibrationInfo"),  json_settings_path + "calibrationsettings.json");
            // services.ConfigureWritableConfig<InclinometerSettings>(Configuration.GetSection("Inclinometers"),   json_settings_path + "inclinometers.json");
            // services.ConfigureWritableConfig<DasSettings>(Configuration.GetSection("DasSettings"),              json_settings_path + "dassettings.json");
            // services.ConfigureWritableConfig<AlgorithmsSettings>(Configuration.GetSection("Algorithms"),        json_settings_path + "algorithmssettings.json");
            // services.ConfigureWritableConfig<UnjackSettings>(Configuration.GetSection("AutoUnjack"),            json_settings_path + "jacksettings.json");
            // services.ConfigureWritableConfig<DrillSettings>(Configuration.GetSection("DrillSettings"),          json_settings_path + "drillsettings.json");
            // services.ConfigureWritableConfig<DrillNormSettings>(Configuration.GetSection("DrillNorms"),         json_settings_path + "drillnorms.json");

            // //Device configs TODO: Fix that mess
            // services.ConfigureWritableConfig<TracksConfig>(Configuration.GetSection("TracksConfiguration"),     json_settings_path + "unitsettings.json");
            // services.ConfigureWritableConfig<JacksSettings>(Configuration.GetSection("JacksConfiguration"),     json_settings_path + "jacksettings.json");
            // services.ConfigureWritableConfig<RotatorSettings>(Configuration.GetSection("Rotator"),              json_settings_path + "unitsettings.json");
            // services.ConfigureWritableConfig<CarouselSettings>(Configuration.GetSection("Carousel"),            json_settings_path + "unitsettings.json");
            // services.ConfigureWritableConfig<ApronSettings>(Configuration.GetSection("Apron"),                  json_settings_path + "unitsettings.json");
            // services.ConfigureWritableConfig<CarvingKeySettings>(Configuration.GetSection("CarvingKey"),        json_settings_path + "unitsettings.json");
            // services.ConfigureWritableConfig<MastSettings>(Configuration.GetSection("Mast"),                    json_settings_path + "unitsettings.json");
            // services.ConfigureWritableConfig<ChainKeySettings>(Configuration.GetSection("ChainKey"),            json_settings_path + "unitsettings.json");

            // services.ConfigureWritableConfig<HppSettings>(Configuration.GetSection("Hpp"),                      json_settings_path + "appsettings.json");
            // services.ConfigureWritableConfig<TelemetrySettings>(Configuration.GetSection("Telemetry"),          json_settings_path + "telemetrysettings.json");
            // services.ConfigureWritableConfig<JacksLimitSwitcheTestSettings>(Configuration.GetSection("JacksLimitSwitcheTestSettings"), json_settings_path + "jacksLimitSwitcheTestSettings.json");
            // services.ConfigureWritableConfig<InputsSettings>(Configuration.GetSection("Inputs"),                json_settings_path + "inputs.json");
            // services.ConfigureWritableConfig<RelationsSettings>(Configuration.GetSection("Relations"),          json_settings_path + "inputs.json");
            // services.ConfigureWritableConfig<UnitSetting>(Configuration.GetSection("Unit"),                     json_settings_path + "appsettings.json");
            // services.ConfigureWritableConfig<StatusesSettings>(Configuration.GetSection("StatusesSettings"),    json_settings_path + "statussettings.json");
            // services.ConfigureWritableConfig<CSBrokerSettings>(Configuration.GetSection("CSBrokerSettings"),    json_settings_path + "statussettings.json");
            // services.ConfigureWritableConfig<AutorizationSettings>(Configuration.GetSection("Autorization"),    json_settings_path + "appsettings.json");
            // services.ConfigureWritableConfig<InputSettings>(Configuration.GetSection("InputSettings"),          json_settings_path + "cansettings.json");
            // services.ConfigureWritableConfig<CanDeviceSetting>(Configuration.GetSection("CanDeviceSetting"),    json_settings_path + "candevices.json");
            // services.ConfigureWritableConfig<AutoSelectHoleSettings>(Configuration.GetSection("AutoSelectHole"),json_settings_path + "autoselecthole_settings.json");

            services.ConfigureWritableConfig<AuthServiceSettings>(Configuration.GetSection("AuthServiceSettings"), json_settings_path + "authServiceSettings.json");
            
            services.ConfigureCustomLoggerWritableConfig<CustomLoggerConfigurationRoot>(Path.Join(json_settings_path, "serilogSettings.json"));
        }

        private static void AddServicesAppsettings(ISettingPathConfig settingsPath, IServiceCollection services, IConfiguration Configuration)
        {
            string json_settings_path = settingsPath.GetSettingsPath() + "appsettings.json";
            //GetConfig don't work after this
            services.ConfigureWritableConfig<AppSettings>(Configuration.GetSection(""), json_settings_path);
        }

        private static void ConfigureWritableConfig<T>(
             this IServiceCollection services,
             IConfigurationSection section,
             string file) where T : class, new()
        {
            _settingsPath.CheckSettingFile(file, false);

            services.Configure<T>(section);
            services.AddSingleton<IWritableConfig<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var environment = provider.GetService<IWebHostEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableJsonConfig<T>(environment, options, configuration, section.Key, file);
            });
        }
        
        public static void ConfigureCustomLoggerWritableConfig<T>(
            this IServiceCollection services,
            string configFilePath
        )
            where T : class, new()
        {
            _settingsPath.CheckSettingFile(configFilePath, false);
            
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException("Config file not found!", configFilePath);
        
            var configuration = new ConfigurationBuilder().AddJsonFile(configFilePath, false, false).Build();

            var loggerAliases = configuration.AsEnumerable().Where(_ => !_.Key.Contains(':')).Select(_ => _.Key).ToArray();
        
            foreach (var loggerAlias in loggerAliases)
            {
                var section = configuration.GetSection(loggerAlias);
                
                services.Configure<T>(section);

                services.AddSingleton<IWritableConfig<T>>(provider =>
                {
                    var environment = provider.GetService<IWebHostEnvironment>();
                    var options = provider.GetService<IOptionsMonitor<T>>();
                    return new WritableJsonConfig<T>(environment, options, configuration, section.Path, configFilePath);
                });
            }

            services.AddSingleton<ICustomLoggerFactory, CustomLoggerFactory>(provider => new CustomLoggerFactory(loggerAliases, provider, configFilePath));
        }
    }
}