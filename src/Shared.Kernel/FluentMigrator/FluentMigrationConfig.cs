using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Reflection;

namespace Shared.Kernel.FluentMigrator;

public static class FluentMigrationConfig
{

    public static ServiceProvider AddFluentMigrationConfig(this IServiceCollection services,
        IConfiguration configuration, Assembly migrationsAssembly)
    {

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var migrationService = new ServiceCollection().AddFluentMigratorCore()
              .ConfigureRunner(rb => rb
              .AddPostgres15_0()
              .WithGlobalConnectionString(LoadConnectionString(configuration, environment))
              .ScanIn(migrationsAssembly).For.Migrations())
              .AddLogging(lb => lb.AddFluentMigratorConsole())
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
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var defaultConnectionString = LoadConnectionString(configuration, environment);

        using (var connection = new NpgsqlConnection(defaultConnectionString))
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

    public static string? LoadConnectionString(IConfiguration configuration, string environment)
    {
        if (!string.IsNullOrWhiteSpace(environment) &&
            !environment.Equals("Production"))
        {
            return configuration["ConnectionStrings:PostgresConnection"];
        }
        else
        {
            var defaultConnectionString = configuration["ConnectionStrings:PostgresConnection"];
            var dataBaseName = configuration["ConnectionStrings:DataBaseName"];

            return defaultConnectionString?.Replace("@DatabaseName", dataBaseName);
        }
    }
}
