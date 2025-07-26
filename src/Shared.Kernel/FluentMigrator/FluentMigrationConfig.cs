using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Shared.Kernel.FluentMigrator;

[ExcludeFromCodeCoverage]
public static class FluentMigrationConfig
{

    public static ServiceProvider AddFluentMigrationConfig(this IServiceCollection services,
        string connectionString, Assembly migrationsAssembly)
    {

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var migrationService = new ServiceCollection().AddFluentMigratorCore()
              .ConfigureRunner(rb => rb
              .AddPostgres15_0()
              .WithGlobalConnectionString(connectionString)
              .ScanIn(migrationsAssembly).For.Migrations())
              .AddLogging(lb => lb.AddFluentMigratorConsole())
              .BuildServiceProvider(false);

        UpdateDatabase(migrationService, connectionString);

        return migrationService;
    }


    private static void UpdateDatabase(IServiceProvider serviceProvider, string connectionString)
    {

        // Garante que o banco de dados exista
        EnsureDatabaseExists(connectionString);

        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        runner.ListMigrations();

        // Execute the migrations
        runner.MigrateUp();


    }


    private static void EnsureDatabaseExists(string connectionString)
    {
        // Pega o nome do banco a ser criado
        var dbName = new NpgsqlConnectionStringBuilder(connectionString).Database;

        // Monta uma connection string para o banco 'postgres', que sempre existe
        var adminConnStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        };

        using (var connection = new NpgsqlConnection(adminConnStringBuilder.ConnectionString))
        {

            connection.Open();

            // Verifica se o banco já existe
            using (var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = @dbName", connection))
            {
                command.Parameters.AddWithValue("dbName", dbName);
                var exists = command.ExecuteScalar() != null;

                if (!exists)
                {
                    // Cria o banco se não existir
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
