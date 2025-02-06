using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Order.Application.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatrService();
    }
    public static void AddMediatrService(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
    }
}
