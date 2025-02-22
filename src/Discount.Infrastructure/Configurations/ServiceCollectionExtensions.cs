using FluentMigrator.Runner;
using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shared.Kernel.Opentelemetry;
using Shared.Kernel.Polices;

namespace Discount.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection PostgresDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHybridRepoNet<CouponsDbContext>(configuration, DbType.PostgreSQL);

        services.ConfigureFluentMigration(configuration);
        services.AddGrafanaSetup(configuration);

        return services;
    }

    public static ServiceProvider ConfigureFluentMigration(this IServiceCollection services, IConfiguration configuration)
    {
        var migrationService = new ServiceCollection().AddFluentMigratorCore()
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
        var retryPolicy = ResiliencePolicies.GetDatabaseRetryPolicy();

        var defaultConnectionString = configuration.GetConnectionString("PostgresConnection");
        var connectionStringWithoutDatabase = RemoveDatabaseFromConnectionString(defaultConnectionString);

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

    private static string RemoveDatabaseFromConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        builder.Database = string.Empty; // Remove o nome do banco de dados
        return builder.ToString();
    }
}
