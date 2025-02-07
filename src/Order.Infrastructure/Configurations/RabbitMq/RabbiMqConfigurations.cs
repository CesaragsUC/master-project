using Message.Broker.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Infrastructure.Consumers;
using System.Diagnostics.CodeAnalysis;

namespace Product.Consumer.Configurations;

[ExcludeFromCodeCoverage]
public static class RabbiMqConfigurations
{
    public static IServiceCollection AddMessageBrokerSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransitSetupConfig(configuration, typeof(OrderCheckoutConsumer));

        return services;
    }
}