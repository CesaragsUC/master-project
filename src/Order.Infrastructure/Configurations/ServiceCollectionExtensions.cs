using FluentMigrator.Runner;
using HybridRepoNet.Configurations;
using HybridRepoNet.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Product.Consumer.Configurations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Order.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureFluentMigration(configuration);
        services.AddMessageBrokerSetup(configuration);
        services.AddHybridRepoNet<OrderDbContext>(configuration,DbType.PostgreSQL);
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
}
