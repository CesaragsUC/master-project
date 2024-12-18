using Domain.Interfaces;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Infrastructure;

[ExcludeFromCodeCoverage]
public class MigratorService : IMigratorService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    public MigratorService( IConfiguration configuration)
    {
        _configuration = configuration;

        _serviceProvider = new ServiceCollection().AddFluentMigratorCore()
             .ConfigureRunner(rb => rb
             .AddPostgres15_0()
             .WithGlobalConnectionString(configuration.GetConnectionString("DefaultConnection"))// Set the connection string
             .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations()) // Define the assembly containing the migrations
             .AddLogging(lb => lb.AddFluentMigratorConsole()) // Enable logging to console in the FluentMigrator way
             .BuildServiceProvider(false);

    }
    public async Task EnsureDatabaseExists()
    {
        var defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
        var connectionStringWithoutDatabase =  RemoveDatabaseFromConnectionString(defaultConnectionString);

        using (var connection = new NpgsqlConnection(connectionStringWithoutDatabase))
        {
            await connection.OpenAsync();

            var dbName = new NpgsqlConnectionStringBuilder(defaultConnectionString).Database;

            using (var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{dbName}'", connection))
            {
                var exists = await command.ExecuteScalarAsync() != null;

                if (!exists)
                {
                    using (var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", connection))
                    {
                       await createCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        await Task.CompletedTask;
    }

    public async Task RollbackAllMigrations()
    {
        using var scope = _serviceProvider.CreateScope();

        // Instantiate the runner
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        // Reverte todas as migrações
        runner.MigrateDown(0);

        await Task.CompletedTask;
    }

    public async Task RollbackLastMigrations(int steps)
    {
        using var scope = _serviceProvider.CreateScope();

        // Instantiate the runner
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        // Reverte as últimas 'steps' migrações
        runner.Rollback(steps);

        await Task.CompletedTask;
    }

    public async Task RollbackMigration(long targetVersion)
    {
        using var scope = _serviceProvider.CreateScope();

        // Instantiate the runner
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        // Reverte para a versão especificada
         runner.MigrateDown(targetVersion);
        
        await Task.CompletedTask;
    }

    public async Task Run()
    {
        // Garante que o banco de dados exista
        await EnsureDatabaseExists();

        using var scope = _serviceProvider.CreateScope();

        // Instantiate the runner
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.ListMigrations();

        // Execute the migrations
        runner.MigrateUp();

        await Task.CompletedTask;
    }

    private static string RemoveDatabaseFromConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        builder.Database = string.Empty; // Remove o nome do banco de dados
        return  builder.ToString();
    }
}
