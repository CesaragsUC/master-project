using Billing.Application.Configurations;

namespace Billing.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddApplicationService();
    }
}
