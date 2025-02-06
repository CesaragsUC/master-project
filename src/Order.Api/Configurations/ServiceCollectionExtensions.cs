using Order.Application.Configurations;

namespace Order.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
    }
}
