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
using Shared.Kernel.Opentelemetry;
using System.Diagnostics.CodeAnalysis;

namespace Order.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddFluentMigrationConfig(configuration, typeof(Migrations.CreateOrderTable).Assembly);
        services.AddMessageBrokerSetup(configuration);
        services.AddGrafanaSetup(configuration);
        services.AddHybridRepoNet<OrderDbContext>(configuration,
            DbType.PostgreSQL,
            (int)HealthCheck.Active,
            FluentMigrationConfig.LoadConnectionString(configuration, environment));
        services.AddKeycloakServices(configuration);

        services.AddScoped<IOrderRepository, OrderRepository>();
    }

    public static void AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationOptions = configuration
                    .GetSection(KeycloakAuthenticationOptions.Section)
                    .Get<KeycloakAuthenticationOptions>();

        var keyCloakConfig = configuration.GetSection("Keycloak:MetadataAddress");

        services.AddKeycloakAuthentication(authenticationOptions!, options =>
        {
            options.MetadataAddress = keyCloakConfig.Value!;
            options.RequireHttpsMetadata = false;
        });


        var authorizationOptions = configuration
                                    .GetSection(KeycloakProtectionClientOptions.Section)
                                    .Get<KeycloakProtectionClientOptions>();

        services.AddKeycloakAuthorization(authorizationOptions!);

    }
}
