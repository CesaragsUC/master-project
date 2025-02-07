using Microsoft.Extensions.DependencyInjection;
using Product.Application.Handlers.Product;
using System.Diagnostics.CodeAnalysis;

namespace Product.Api.Tests;

[ExcludeFromCodeCoverage]
public abstract class BaseConfig
{
    protected void InitializeMediatrService()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateProductHandler).Assembly))
            .BuildServiceProvider();

    }
}
