using Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.Handlers.Product;

namespace Product.Api.Tests;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
      IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly ProductDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

        DbContext = _scope.ServiceProvider
            .GetRequiredService<ProductDbContext>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }

    protected void InitializeMediatrService()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateProductHandler).Assembly))
            .BuildServiceProvider();

    }
}