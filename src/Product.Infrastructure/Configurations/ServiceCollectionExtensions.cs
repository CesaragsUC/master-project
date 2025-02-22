using Azure.Storage.Blobs;
using FluentMigrator.Runner;
using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Product.Infrastructure.Configurations.Azure;
using Product.Infrastructure.RabbitMq;
using Serilog;
using Shared.Kernel.Opentelemetry;
using Shared.Kernel.Polices;
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
        services.AddMessageBrokerSetup(configuration);
        services.AddHybridRepoNet<ProductDbContext>(configuration, DbType.PostgreSQL);
        services.AddGrafanaSetup(configuration);

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

        ResiliencePolicies.GetDatabaseRetryPolicy().ExecuteAsync(() => Task.Run(() => UpdateDatabase(migrationService, configuration)));

        return migrationService;
    }


    private static async Task UpdateDatabase(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        // Garante que o banco de dados exista
        await EnsureDatabaseExists(configuration);

        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

         runner.ListMigrations();

        // Execute the migrations
         runner.MigrateUp();
    }

    private static async Task EnsureDatabaseExists(IConfiguration configuration)
    {
        try
        {
            var retryPolicy = ResiliencePolicies.GetDatabaseRetryPolicy();

            var defaultConnectionString = configuration.GetConnectionString("PostgresConnection");
            var connectionStringWithoutDatabase = RemoveDatabaseFromConnectionString(defaultConnectionString!);

            await retryPolicy.ExecuteAsync(async () =>
            {
                using (var connection = new NpgsqlConnection(connectionStringWithoutDatabase))
                {
                    await connection.OpenAsync();

                    var dbName = new NpgsqlConnectionStringBuilder(defaultConnectionString).Database;

                    using (var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{dbName}'", connection))
                    {
                        var exists = command.ExecuteScalarAsync() != null;

                        if (!exists)
                        {
                            using (var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", connection))
                            {
                                await createCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unexpected error occurred while ensuring the database exists");
            throw;
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
}
