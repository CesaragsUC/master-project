using Api.Gateway.Dtos;
using Api.Gateway.Services;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Kernel.Models;


namespace Api.Gateway.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<AuthenticationService>();
        services.AddHttpClient();

        services.AddJwtServices(configuration);
        services.AddOpenTelemetryServices(configuration);
        services.AddAuthServices(configuration);
        services.AddCors(configuration);

    }

    public static IServiceCollection AddOceloConfigurations(this IServiceCollection services, ConfigurationManager configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"ocelot.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"ocelot.{environment}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        Console.WriteLine($"Api.Gateway Environment: {environment}");

        services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());

        return services;
    }

    public static void AddJwtServices(this IServiceCollection services,IConfiguration configuration)
    {
       var _keycloakSettings = configuration.GetSection("KeycloakSettings").Get<KeycloakSettings>();

        services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = _keycloakSettings?.AuthServerUrl;
            options.Audience = _keycloakSettings?.Resource;
            options.RequireHttpsMetadata = false;
        });

    }

    public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EndPointUri>(configuration.GetSection("EndPointUri"));

    }

    public static void AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        var _fontUri = configuration.GetSection("FrontEndUri").Get<FrontEndUri>();  

        services.AddCors(options =>
        {
            options.AddPolicy(_fontUri.Name!, policy =>
            {
                 policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            });

            options.AddPolicy("Production", policy =>
            {
                policy.WithOrigins(_fontUri.Uri!)
                      .AllowAnyHeader()
                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                      .AllowAnyMethod();
            });
        });

    }

    public static void AddOpenTelemetryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jaegerConfig = configuration.GetSection("OpenTelemetry");
        var serviceName = jaegerConfig.GetValue<string>("ServiceName");
        var jaegerHost = jaegerConfig.GetValue<string>("Jaeger:AgentHost");
        var jaegerPort = jaegerConfig.GetValue<int>("Jaeger:AgentPort");


        services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(serviceName))
        .WithTracing(builder =>
        {
            builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                  .AddJaegerExporter(options =>
                  {
                      options.AgentHost = jaegerHost;
                      options.AgentPort = jaegerPort;
                  });
        });

    }
}
