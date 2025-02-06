using Message.Broker.RabbitMq;

namespace Basket.Api.Configurations;

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