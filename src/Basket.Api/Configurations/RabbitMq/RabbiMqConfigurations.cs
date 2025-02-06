using Message.Broker.RabbitMq;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Configurations;

[ExcludeFromCodeCoverage]
public static class RabbiMqConfigurations
{
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