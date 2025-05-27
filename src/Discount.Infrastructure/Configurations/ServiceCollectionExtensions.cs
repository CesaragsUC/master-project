using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.FluentMigrator;
using Shared.Kernel.KeyCloackConfig;
using Shared.Kernel.Opentelemetry;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        services.AddHybridRepoNet<CouponsDbContext>(
            configuration,
            DbType.PostgreSQL,
            (int)HealthCheck.Active,
            FluentMigrationConfig.LoadConnectionString(configuration, environment));

        services.AddFluentMigrationConfig(configuration, typeof(Migrations.CoupounTableCreate).Assembly);
        services.AddGrafanaSetup(configuration);
        services.AddKeycloakServices(configuration);

        return services;
    }
}
