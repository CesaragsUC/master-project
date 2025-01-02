using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Domain.Abstractions;

namespace Product.Services.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }

}
