using Order.Application.Configurations;
using Order.Infrastructure.RabbitMq;

namespace Order.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddScoped<IQueueService, QueueService>();
    }
}
