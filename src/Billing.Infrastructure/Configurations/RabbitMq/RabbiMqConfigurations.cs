using Billing.Infrastructure.Consumers;
using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Message.Broker.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Shared.Kernel.Opentelemetry;
using Billing.Infrastructure.Configurations;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

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
        services.AddGrafanaSetup(configuration);
        services.AddKeycloakServices(configuration);
        services.ConfigureFluentMigration(configuration);

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