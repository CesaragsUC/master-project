using Message.Broker.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class RabbiMqConfigurations
{
    public static IServiceCollection AddMessageBrokerSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddMassTransitSetupConfig(configuration);

        return services;
    }
}