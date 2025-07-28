using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Domain.Abstraction;
using Order.Infrastructure.Repository;
using Product.Consumer.Configurations;
using Shared.Kernel.FluentMigrator;
using Shared.Kernel.KeyCloackConfig;
using Shared.Kernel.Opentelemetry;
using System.Diagnostics.CodeAnalysis;

namespace Order.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddFluentMigrationConfig(FluentMigrationConfig.LoadConnectionString(configuration, environment),
            typeof(Migrations.CreateOrderTable).Assembly);
        services.AddMessageBrokerSetup(configuration);
        services.OpenTelemetryConfig(configuration);
        services.AddHybridRepoNet<OrderDbContext>(configuration,
            DbType.PostgreSQL,
            (int)HealthCheck.Active,
            FluentMigrationConfig.LoadConnectionString(configuration, environment));
        services.AddKeycloakServices(configuration);

        services.AddScoped<IOrderRepository, OrderRepository>();
    }

}
