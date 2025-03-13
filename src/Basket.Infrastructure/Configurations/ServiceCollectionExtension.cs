using EasyMongoNet.Exntesions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Shared.Kernel.Opentelemetry;
using StackExchange.Redis;


namespace Basket.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration);
        services.AddMessageBrokerSetup(configuration);
        services.AddGrafanaSetup(configuration);
        services.AddRedisConfiguration(configuration);
    }

    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEasyMongoNet(configuration);
    }

    public static void AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetSection("Redis:ConnectionString").Value!;
        string user = configuration.GetSection("Redis:User").Value!;
        string pass = configuration.GetSection("Redis:Password").Value!;
        int port = int.Parse(configuration.GetSection("Redis:Port").Value!);

        IConnectionMultiplexer connectionMultiplexer =
            ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { { connectionString, port } },
                User = user,
                Password = pass
            });

        services.AddSingleton(connectionMultiplexer);

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory =
                () => Task.FromResult(connectionMultiplexer);
        });
    }

}
