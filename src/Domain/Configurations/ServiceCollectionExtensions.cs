using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Product.Domain.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddMediatrService(this IServiceCollection services)
    {
        //Registra todos os handlers do MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
    }
}
