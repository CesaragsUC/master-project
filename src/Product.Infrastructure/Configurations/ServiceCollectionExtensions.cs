using Azure.Storage.Blobs;
using EasyMongoNet.Exntesions;
using FluentMigrator.Runner;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Product.Infrastructure.Configurations.Azure;
using Product.Infrastructure.RabbitMq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfra(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.ConfigureFluentMigration(configuration);
        services.AddKeycloakServices(configuration);
        services.AddAzureBlobServices(configuration);
        services.AddOpenTelemetryServices(configuration);
        services.MongoDbService(configuration);
        services.AddMessageBrokerSetup(configuration);

        return services;
    }

    public static ServiceProvider ConfigureFluentMigration(this IServiceCollection services, IConfiguration configuration)
    {
        var migrationService =  new ServiceCollection().AddFluentMigratorCore()
              .ConfigureRunner(rb => rb
              .AddPostgres15_0()
              .WithGlobalConnectionString(configuration.GetConnectionString("PostgresConnection"))// Set the connection string
              .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations()) // Define the assembly containing the migrations
              .AddLogging(lb => lb.AddFluentMigratorConsole()) // Enable logging to console in the FluentMigrator way
              .BuildServiceProvider(false);

        UpdateDatabase(migrationService, configuration);

        return migrationService;
    }


    private static void UpdateDatabase(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        // Garante que o banco de dados exista
         EnsureDatabaseExists(configuration);

        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

         runner.ListMigrations();

        // Execute the migrations
         runner.MigrateUp();
    }

    private static void EnsureDatabaseExists(IConfiguration configuration)
    {
        var defaultConnectionString = configuration.GetConnectionString("PostgresConnection");
        var connectionStringWithoutDatabase = RemoveDatabaseFromConnectionString(defaultConnectionString);

        using (var connection = new NpgsqlConnection(connectionStringWithoutDatabase))
        {
            connection.Open();

            var dbName = new NpgsqlConnectionStringBuilder(defaultConnectionString).Database;

            using (var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{dbName}'", connection))
            {
                var exists = command.ExecuteScalar() != null;

                if (!exists)
                {
                    using (var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", connection))
                    {
                        createCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
    private static string RemoveDatabaseFromConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        builder.Database = string.Empty; // Remove o nome do banco de dados
        return builder.ToString();
    }

    public static void AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationOptions = configuration
                    .GetSection(KeycloakAuthenticationOptions.Section)
                    .Get<KeycloakAuthenticationOptions>();

        var metaDataConfig = configuration.GetSection("Keycloak:MetadataAddress");

        services.AddKeycloakAuthentication(authenticationOptions!, options =>
        {
            options.MetadataAddress = metaDataConfig.Value!;
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

    public static void AddOpenTelemetryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jaegerConfig = configuration.GetSection("OpenTelemetry");
        var serviceName = jaegerConfig.GetValue<string>("ServiceName");
        var jaegerHost = jaegerConfig.GetValue<string>("Jaeger:AgentHost");
        var jaegerPort = jaegerConfig.GetValue<int>("Jaeger:AgentPort");


        services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(serviceName))
        .WithTracing(builder =>
        {
            builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                  .AddJaegerExporter(options =>
                  {
                      options.AgentHost = jaegerHost;
                      options.AgentPort = jaegerPort;
                  });
        });

    }

    public static IServiceCollection MongoDbService(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddEasyMongoNet(configuration);

        return services;
    }
}
