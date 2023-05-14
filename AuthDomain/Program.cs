using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using AuthBLL.Bearer.Auth.Auth.JWT;
using AuthBLL.Services.Repository;
using AuthBLL.Services.SmtpEmailSender;
using AuthBLL.Services.User;
using AuthDomain;
using AuthDomain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MobileDrill.DataBase.Data;
using Serilog;
using Serilog.Enrichers.WithCaller;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Swashbuckle.AspNetCore.SwaggerUI;
using AuthDomain.Hubs.AppAuth;

InitRootLoger();
Log.Information("Application root starting...");

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configurationManager = builder.Configuration;
IWebHostEnvironment env = builder.Environment;
ConfigureHostBuilder host = builder.Host;

Log.Information("InitConfigurationManager...");
InitConfigurationManager(configurationManager,env);
Log.Information("InitHostSettings...");
InitHostSettings(host);

// Add services to the container.
IServiceCollection serviceCollection = builder.Services;
Log.Information("AddConfigServices...");
serviceCollection.AddConfigServices(); //Добавляем файлы json
Log.Information("InitSettings...");
serviceCollection.InitSettings(configurationManager,env);


var authOptionsConfiguration = configurationManager.GetSection("Auth");
serviceCollection.Configure<AuthOptions>(authOptionsConfiguration);
serviceCollection.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
serviceCollection.AddEndpointsApiExplorer();

// X509Certificate2 cert = new X509Certificate2("localhost.cer");

// X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
// store.Open(OpenFlags.ReadOnly);
// foreach (X509Certificate2 certificate in store.Certificates)
// {
//         // Экспорт сертификата в формате PEM
//     byte[] certData = certificate.Export(X509ContentType.Cert);
    
//     // Match the CN field with regex
//     Match match = Regex.Match(certificate.Subject, @"CN=([^,]+)");
//     if (match.Success)
//     {
//         // Extract the value of the CN field
//         string cn = match.Groups[1].Value.Trim();
//         File.WriteAllBytes($"{cn}.cer", certData);
//         // Output: Sergei Smoglyuk
//     }

//     Console.WriteLine($"Subject: {certificate.Subject}");
//     Console.WriteLine($"Issuer: {certificate.Issuer}");
//     Console.WriteLine($"Thumbprint: {certificate.Thumbprint}");
//     Console.WriteLine();

   
// }
// store.Close();

var app = builder.Build();


Log.Information($"Environment: {app.Environment.EnvironmentName}");

if (app.Environment.IsDevelopment())
{
    Log.Information("Add Swagger & SwaggerUI");
    // app.UseSwagger().UseSwaggerUI(c =>
    // {
    //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    //     // c.RoutePrefix = string.Empty;
    //     // c.EnableValidator(null);
    //     // c.EnableFilter(null);
    //     // c.DocExpansion(DocExpansion.None);
    //     // c.DefaultModelsExpandDepth(-1);
    //     // c.DisplayRequestDuration();
    //     // c.EnableDeepLinking();
    //     // c.ShowExtensions();
    // });
    app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
else
{
    app.UseHsts();
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "[{httpContextTraceIdentifier}][{httpContextRequestScheme}] {httpContextRequestProtocol} {httpContextRequestMethod} {httpContextRequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("httpContextTraceIdentifier", Activity.Current?.Id ?? httpContext.TraceIdentifier);
        diagnosticContext.Set("httpContextConnectionId", httpContext.Connection.Id);
        diagnosticContext.Set("httpContextConnectionRemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("httpContextConnectionRemotePort", httpContext.Connection.RemotePort);
        diagnosticContext.Set("httpContextRequestHost", httpContext.Request.Host);
        diagnosticContext.Set("httpContextRequestPath", httpContext.Request.Path);
        diagnosticContext.Set("httpContextRequestProtocol", httpContext.Request.Protocol);
        diagnosticContext.Set("httpContextRequestIsHttps", httpContext.Request.IsHttps);
        diagnosticContext.Set("httpContextRequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("httpContextRequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("httpContextRequestContentType", httpContext.Request.ContentType);
        diagnosticContext.Set("httpContextRequestContentLength", httpContext.Request.ContentLength);
        diagnosticContext.Set("httpContextRequestQueryString", httpContext.Request.QueryString);
        diagnosticContext.Set("httpContextRequestQuery", httpContext.Request.Query);
        diagnosticContext.Set("httpContextRequestHeaders", httpContext.Request.Headers);
        diagnosticContext.Set("httpContextRequestCookies", httpContext.Request.Cookies);
    };
});
app.UseExceptionHandler("/Error");
app.UseRouting();
app.UseAuthentication();
app.UseResponseCaching();
app.UseAuthorization();
app.UseCors();
app.UseWebSockets();
app.MapControllers();
app.UseHttpsRedirection();
app.MapHub<AppHub>("/appHub");
app.MapHub<TelemetryHub>("/TelemetryHub");

SyncOrCreateDbAsync(app);

try
{
    Log.Information("Application setting is finished...");
    app.Run();
    Log.Information("Application stopping...");
}
catch (Exception e)
{
    Log.Fatal(e, "An unhandled exception occured during bootstrapping!");
}
finally
{
    Log.Information("Flushing logs...");
    Log.CloseAndFlush();
}

static void InitRootLoger()
{
    var settingsPath = new SettingPathConfig();
    var fileName = "serilogSettings.json";
    var check = settingsPath.CheckSettingFile(fileName);
    var test = settingsPath.GetSettingsPath();

    var configurationRoot = new ConfigurationBuilder().SetBasePath(settingsPath.GetSettingsPath()).AddJsonFile(fileName, true, false).Build();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configurationRoot, "Bootstrap")
        .Enrich.WithCaller(true)
        .CreateBootstrapLogger();
}

static void InitConfigurationManager(ConfigurationManager configurationManager,IWebHostEnvironment env)
{
    
    configurationManager.AddEnvironmentVariables();

    if (env.IsDevelopment())
    {
        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        configurationManager.AddUserSecrets(appAssembly, true);
    }
}

static void InitHostSettings(ConfigureHostBuilder host)
{
    
    host.UseSerilog((context, services, configuration) =>
    {
        var settingsPath = new SettingPathConfig();
        var fileName = "serilogSettings.json";
        var check = settingsPath.CheckSettingFile(fileName);
        var test = settingsPath.GetSettingsPath();

        var configurationRoot = new ConfigurationBuilder().SetBasePath(settingsPath.GetSettingsPath()).AddJsonFile(fileName, true, false).Build();

        configuration
            .ReadFrom.Configuration(configurationRoot, "Default")
            .Enrich.WithCaller(true);
    });

}

void SyncOrCreateDbAsync(WebApplication app)
{
    Log.Information("Starting synchronization with the global database...");
    var services = app.Services;
    try
    {
        var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
        using var scope =  scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }

        Log.Information("Finishing synchronization \"AuthDbContext\" with the global database...");
    }
    catch (Exception ex)
    {
        Log.Error($"An error occurred while creating the database: {ex.Message}");
    }
}