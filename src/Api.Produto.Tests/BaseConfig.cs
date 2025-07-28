using Microsoft.Extensions.DependencyInjection;
using Product.Application.Handlers.Product;
using System.Diagnostics.CodeAnalysis;
using Testcontainers.PostgreSql;

namespace Product.Api.Tests;

[ExcludeFromCodeCoverage]
public abstract class BaseConfig : IAsyncLifetime
{
    protected void InitializeMediatrService()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateProductHandler).Assembly))
            .BuildServiceProvider();

    }

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.StopAsync();
    }

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
    .WithImage("postgres:16")
    .WithUsername("admin")
    .WithPassword("Teste@123")
    .Build();
}
