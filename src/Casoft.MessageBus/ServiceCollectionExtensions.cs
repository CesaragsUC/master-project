using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Casoft.MessageBus;

public static class ServiceCollectionExtensions
{
    public static void AddMassTransientService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMessageBus, MessageBus>();

        var host = configuration.GetSection("RabbitMqTransport:Host").Value;
        var user = configuration.GetSection("RabbitMqTransport:User").Value;
        var pass = configuration.GetSection("RabbitMqTransport:Pass").Value;

        services.AddMassTransit(m =>
        {
            m.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(host, "/", c =>
                {
                    c.Username(user);
                    c.Password(pass);
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });
    }
}
