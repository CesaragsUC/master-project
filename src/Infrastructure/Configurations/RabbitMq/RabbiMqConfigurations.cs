using Message.Broker.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Infrastructure.Consumers;
using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.RabbitMq;

[ExcludeFromCodeCoverage]
public static class RabbiMqConfigurations
{
    public static IServiceCollection AddMessageBrokerSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddMassTransitSetupConfig(configuration,
        typeof(ProductAddedConsumer),
        typeof(ProdutctDeletedConsumer),
        typeof(ProdutctUpdatedConsumer));

        return services;
    }
}
