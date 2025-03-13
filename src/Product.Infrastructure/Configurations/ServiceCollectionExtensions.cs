using Azure.Storage.Blobs;
using EasyMongoNet.Exntesions;
using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Product.Infrastructure.Configurations.Azure;
using Product.Infrastructure.RabbitMq;
using Shared.Kernel.FluentMigrator;
using Shared.Kernel.Opentelemetry;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfra(this IServiceCollection services,
        IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddFluentMigrationConfig(configuration, typeof(Product.Infrastructure.Migrations.Inicio).Assembly);
        services.AddKeycloakServices(configuration);
        services.AddAzureBlobServices(configuration);
        services.AddMessageBrokerSetup(configuration);
        services.AddHybridRepoNet<ProductDbContext>(configuration,
            DbType.PostgreSQL,
            (int)HealthCheck.Active,
            FluentMigrationConfig.LoadConnectionString(configuration, environment));
        services.AddGrafanaSetup(configuration);
        services.MongoDbService(configuration);

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


    public static void AddAzureBlobServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobContainers>(configuration.GetSection("BlobContainers"));

        services.AddSingleton(provider =>
        {
            var blobContainers = provider.GetRequiredService<IOptions<BlobContainers>>().Value;
            return new BlobServiceClient(blobContainers.ConnectionStrings);
        });
    }

    public static IServiceCollection MongoDbService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEasyMongoNet(configuration);

        return services;
    }

    private static string? GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:PostgresConnection"];
        var dataBaseName = configuration["ConnectionStrings:DataBaseName"];

        return connectionString?.Replace("@DatabaseName", dataBaseName);
    }
}
