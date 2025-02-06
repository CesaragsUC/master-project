using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Billing.Application.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationService(this IServiceCollection services)
    {
        //Registra todos os handlers do MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
    }
}
