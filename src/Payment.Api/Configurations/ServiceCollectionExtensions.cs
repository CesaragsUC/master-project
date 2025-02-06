using Billing.Application.Configurations;
using Billing.Infrastructure.Configurations.RabbitMq;

namespace Billing.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddApplicationService();
        services.AddScoped<IQueueService, QueueService>();
    }
}
