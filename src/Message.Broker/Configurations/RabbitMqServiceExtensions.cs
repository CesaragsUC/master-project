using MassTransit;
using Message.Broker.Abstractions;
using Message.Broker.RabbitMq;
using Message.Broker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Message.Broker.Configurations;

[ExcludeFromCodeCoverage]
public static class RabbitMqServiceExtensions
{

    public static IServiceCollection AddMassTransitFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRabbitMqBusFactoryConfigurator>? extraConfig = null)
    {
        services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMqTransportOptions"));

        var rabbitMqOptions = new RabbitMqConfig();
        configuration.GetSection("RabbitMqTransportOptions").Bind(rabbitMqOptions);

        services.AddSingleton<IRabbitMqService, RabbitMqService>();
        services.AddHostedService<RabbitMqHostedService>();

        if (extraConfig != null)
        {
            services.AddSingleton(extraConfig);
        }

        return services;
    }


}
