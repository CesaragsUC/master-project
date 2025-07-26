using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.FluentMigrator;
using System;
using System.Diagnostics.CodeAnalysis;
using Testcontainers.PostgreSql;



public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithUsername("admin")
        .WithName("integration-test-postgres")
        .WithPassword("Teste@123")
        .WithDatabase("Products") // Recomendo nome exclusivo pra testes
        .WithPortBinding(5432, true) // Deixe o sistema escolher a porta livre
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                var descriptorType = typeof(DbContextOptions<ProductDbContext>);

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == descriptorType);

                if (descriptor != null)
                {
                    // Remove the existing DbContext registration
                    services.Remove(descriptor);
                }

                // Add the PostgreSQL container connection string
                services.AddDbContext<ProductDbContext>(options =>
                {
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
                });

                services.AddFluentMigrationConfig(_postgreSqlContainer.GetConnectionString(),
                    typeof(Product.Infrastructure.Migrations.Inicio).Assembly);

            }
        });
    }

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _postgreSqlContainer.StopAsync();
    }
}
