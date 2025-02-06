using EasyMongoNet.Exntesions;
using Message.Broker.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration);
        services.AddMassTransitSetup(configuration);
    }
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEasyMongoNet(configuration);
    }

    public static IServiceCollection AddMassTransitSetup(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMqTransportOptions"));

        var rabbitMqOptions = new RabbitMqConfig();
        configuration.GetSection("RabbitMqTransportOptions").Bind(rabbitMqOptions);

        return services;
    }
}
