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
        services.AddKeycloakServices(configuration);
        services.AddFluentMigrationConfig(configuration, typeof(Migrations.CreateTablePayment).Assembly);
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        return services;
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
