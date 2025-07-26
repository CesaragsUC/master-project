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
        .WithPassword("Teste@123")
        .WithDatabase("Products") // Recomendo nome exclusivo pra testes
        .WithPortBinding(0, true) // Deixe o sistema escolher a porta livre
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var mappedPort = _postgreSqlContainer.GetMappedPublicPort(5432);
                var connectionString = $"Host=localhost;Port={mappedPort};Database=Products;Username=admin;Password=Teste@123;";

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
                    options.UseNpgsql(connectionString);
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
