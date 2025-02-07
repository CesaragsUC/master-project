using Billing.Application.Configurations;
using Billing.Infrastructure.Configurations.RabbitMq;

namespace Billing.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddApplicationService(configuration);
        services.AddScoped<IQueueService, QueueService>();
    }
}
