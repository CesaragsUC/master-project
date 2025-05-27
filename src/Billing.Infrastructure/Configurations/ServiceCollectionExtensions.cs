using Billing.Domain.Abstractions;
using Billing.Infrastructure.Consumers;
using Billing.Infrastructure.Repository;
using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Message.Broker.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.FluentMigrator;
using Shared.Kernel.KeyCloackConfig;
using Shared.Kernel.Opentelemetry;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{



    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddMassTransitSetupConfig(configuration,
            typeof(PaymentConsumer),
            typeof(PaymentConfirmedConsumer),
            typeof(PaymentFailedConsumer));

        services.AddHybridRepoNet<BillingContext>(
            configuration,
            DbType.PostgreSQL,
            (int)HealthCheck.Active,
            FluentMigrationConfig.LoadConnectionString(configuration, environment));

        services.AddGrafanaSetup(configuration);
        services.AddFluentMigrationConfig(configuration, typeof(Migrations.CreateTablePayment).Assembly);
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddKeycloakServices(configuration);
        return services;
    }

}
