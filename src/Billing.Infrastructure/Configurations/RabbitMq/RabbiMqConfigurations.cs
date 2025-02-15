using Billing.Infrastructure.Consumers;
using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Message.Broker.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Infrastructure.RabbitMq;

[ExcludeFromCodeCoverage]
public static class RabbiMqConfigurations
{

    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMassTransitSetupConfig(configuration,
            typeof(PaymentConsumer),
            typeof(PaymentConfirmedConsumer),
            typeof(PaymentFailedConsumer));

        services.AddHybridRepoNet<BillingContext>(configuration, DbType.PostgreSQL);

        return services;
    }

}