using System.Diagnostics;
using AuthDAL.Enums;
using AuthDAL.Models;
using Microsoft.OpenApi.Models;
using MobileDrill.DataBase.Data;
using AuthBLL.Filters;
using Microsoft.EntityFrameworkCore;
using AuthService.Authorization;
using AuthDAL.Settings;
using AuthService.Authentication;
using AuthBLL.Converters;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using AuthBLL.Services.Repository;
using AuthBLL.Services.User;
using AuthBLL.Services.SmtpEmailSender;
using System.Net.Mail;
using AuthBLL.Handlers;
using AuthBLL.Services;
using AuthBLL.Services.Advanced;
using AuthService.Services.Advanced;
using AuthBLL.Repository.Base;
using AuthDAL.Auth.AuthService;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using AuthService.ConfigureNamedOptions;
using AuthServiceV2.ConfigureOptions;

namespace AuthDomain
{
    public static partial class ServiceProviderExtensions
    {
        public static void InitSettings(this IServiceCollection services,IConfiguration configurationManager,IWebHostEnvironment env)
        {
            services.InitCors();
            services.AddSwagger();
            
            services.AddCustomDbContext(configurationManager, env);
            services.AddTransientServices(configurationManager);
            services.AddScopedServices(configurationManager);
            services.AddSingeltonServices(configurationManager);

            services.AddConfigureSignalR();
            services.AddHttpContextAccessor();
            // services.AddHttpClient<ICoreServiceClient, CoreServiceMmsClient>();
            services.AddConfigureMvc();
            services.AddConfigureSignalR();
            services.AddConfigureControllers();
            services.AddConfigureAuthentication();
            services.AddConfigureAuthorization(configurationManager);
          
            services.AddHostedService(configurationManager);

        }

        /// <summary>
        ///     Method to initialization CORS policy
        /// </summary>
        private static IServiceCollection InitCors(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(_ => true);
                    // policy.AllowAnyMethod()
                    //     .AllowAnyHeader()
                    //     .AllowCredentials()
                    //     .SetIsOriginAllowed(_ => true)
                    //     .WithOrigins("http://localhost:4200/", "http://localhost:8080/");
                })
            );

            return serviceCollection;
        }

        /// <summary>
        ///     Method to add DbContext
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        private static IServiceCollection AddCustomDbContext(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            IWebHostEnvironment env
        )
        {
            // serviceCollection.AddDbContext<AuthDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b=> b.MigrationsAssembly("AuthDAL")));
            //Context lifetime MUST BE ServiceLifetime.Scoped, for usage in singletons, use IDbContextFactory<AppDbContext> instead
            //Options must be ServiceLifetime.Singleton in order to be consumed in a DbContextFactory which is a singleton
            serviceCollection.AddDbContext<AuthDbContext>(options =>
            {
                options
                    .UseSqlite(
                        configuration.GetConnectionString("AuthConnection"),
                        _ => _.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

                if (env.IsDevelopment()) options.EnableSensitiveDataLogging().EnableDetailedErrors();
            }, ServiceLifetime.Scoped, ServiceLifetime.Singleton);


            /*
             * Use IDbContextFactory<AppDbContext> in singletons
             * Then pass DbContext returned by .CreateDbContext() into ctor of AppDbContextAction and RepositoryBase<>
             * This way you are able to use one DbContext over multiple Repositories in Singletons (giving ability to do one transaction)
             */
            serviceCollection.AddDbContextFactory<AuthDbContext>(options =>
            {
                options
                    .UseSqlite(
                        configuration.GetConnectionString("AuthConnection"),
                        _ => _.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

                if (env.IsDevelopment()) options.EnableSensitiveDataLogging().EnableDetailedErrors();
            });

            return serviceCollection;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1",
                    Description = "An API of ASP.NET Core MobileDrill",
                });

                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Name = "Authorization"
                    });

                c.OperationFilter<AuthorizeCheckOperationFilter>();

                c.EnableAnnotations();
            });
        
            return serviceCollection;
        }

        public static void InitLogging(this IServiceCollection services)
        {
            var logLevel = Serilog.Events.LogEventLevel.Error;

            string logsFolder = "logs";

            string loggerOutputTemplate =
                "-------------------------------------\n[{ProjectSignature}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{FilePath}/{MemberName}:{LineNumber}]\n[{Tag}][{Level:u3}]: {Message:lj}{NewLine}{Exception}";
            string currentDate = DateTime.Today.ToString("dd.MM.yyyy");

            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", logLevel)
                .WriteTo.Console(outputTemplate: loggerOutputTemplate)
                .WriteTo.Map("FileName", "main", (name, wt) =>
                {
                    wt.File(path: $"{logsFolder}/{currentDate}/{name}.log", outputTemplate: loggerOutputTemplate);
                })
                .CreateLogger();
        }

        public static void LoggingCheckConfigurationFile(this IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var settings = configuration.GetSection("RIT_Serilog").Get<Logging.RIT_Serilog>();

            if(settings == null)
            {
                throw new Exception("Don't find \"RIT_Serilog\" in logSettings.json file.");
            }

            Logging.Log._settings = settings;
        }

        private static IServiceCollection AddSingeltonServices(this IServiceCollection serviceCollection, IConfiguration Configuration)
        {
           
            var smtpClient = new SmtpClient("smtp.mail.ru")
            {
                Host = "smtp.mail.ru",
                Port = 587,
                Credentials = new System.Net.NetworkCredential("madjita@mail.ru", "BPJ61yceJQBXpidNiWPA"),
                EnableSsl = true,
                UseDefaultCredentials = false,

            };
            serviceCollection.AddSingleton(smtpClient);

            serviceCollection.AddSingleton<IConfigureOptions<AuthenticationOptions>, ConfigureAuthenticationOptions>();
            serviceCollection.AddSingleton<IConfigureOptions<JsonWebTokenAuthenticationSchemeOptions>, ConfigureJwtBearerOptions>();

            return serviceCollection;
        }

        private static IServiceCollection AddTransientServices(this IServiceCollection services, IConfiguration Configuration)
        {
            return services;
        }

        private static IServiceCollection AddScopedServices(this IServiceCollection serviceCollection, IConfiguration Configuration)
        {
            //TODO Тестовый сервис для взаимодействия с Пользователем.
            serviceCollection.AddScoped<IAuthHandler,AuthHandler>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IEmailSender, SmtpEmailSender>();
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            serviceCollection.AddScoped(typeof(IRepositoryBase<,>),typeof(RepositoryBase<,>));

            //Extentions Contexts
            serviceCollection.AddScoped(typeof(AppDbContextAction<>));
            serviceCollection.AddScoped(typeof(IAppDbContextEntityAction<>), typeof(AppDbContextAction<>));
            serviceCollection.AddScoped(typeof(IAppDbContextTransactionAction<>), typeof(AppDbContextAction<>));

             //Services for Auth
            serviceCollection.AddScoped<IWarningService, WarningService>();
            serviceCollection.AddScoped<IAccumulateActionsService, AccumulateActionsService>();
            serviceCollection.AddScoped<IJsonWebTokenAdvancedService, JsonWebTokenAdvancedService>();
            serviceCollection.AddScoped<IAccountAdvancedService, AccountAdvancedService>();

            return serviceCollection;
        }
        private static IServiceCollection AddHostedService(this IServiceCollection serviceCollection, IConfiguration Configuration)
        {

            return serviceCollection;
        }

        public static IServiceCollection AddConfigureMvc(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddMvc()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // options.SuppressModelStateInvalidFilter = true;
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errorModelResult = new ErrorModelResult
                        {
                            TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
                        };

                        foreach (var modelError in context.ModelState.Values.SelectMany(modelStateValue => modelStateValue.Errors))
                            errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.ModelState, modelError.ErrorMessage));

                        return new BadRequestObjectResult(errorModelResult);
                    };
                });
            
            return serviceCollection;
        }
        
        public static void AddConfigureControllers(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddControllers(options => { options.Filters.Add<HttpResponseExceptionFilter>(); })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;

                    options.JsonSerializerOptions.Converters.Add(new StringTrimmingConverter());
                    // options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });
        }
        
        public static IServiceCollection AddConfigureSignalR(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSignalR(options => { options.EnableDetailedErrors = true; }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNameCaseInsensitive = false;
                options.PayloadSerializerOptions.Converters.Add(new StringTrimmingConverter());
                options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
            
            return serviceCollection;
        }
        
        public static IServiceCollection AddConfigureAuthentication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthentication()
                .AddScheme<DefaultAuthenticationSchemeOptions, DefaultAuthenticationHandler>(AuthenticationSchemes.Default.ToString(), null!)
                .AddScheme<AccessTokenAuthenticationSchemeOptions, AccessTokenAuthenticationHandler>(AuthenticationSchemes.AccessToken.ToString(), null!)
                .AddScheme<JsonWebTokenAuthenticationSchemeOptions, JsonWebTokenAuthenticationHandler>(AuthenticationSchemes.JsonWebToken.ToString(), null!);
            
            return serviceCollection;
        }
        
        public static IServiceCollection AddConfigureAuthorization(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var authServiceSettings = configuration.GetSection(nameof(AuthServiceSettings)).Get<AuthServiceSettings>();

            serviceCollection.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(AccessTokenSystemAuthorizationRequirement),
                    policy =>
                    {
                        policy.Requirements.Add(new AccessTokenSystemAuthorizationRequirement(authServiceSettings!.SignalRSystemAccessToken));
                        policy.AuthenticationSchemes.Add(AuthenticationSchemes.AccessToken.ToString());
                    });
            });
            
            return serviceCollection;
        }
    }
}