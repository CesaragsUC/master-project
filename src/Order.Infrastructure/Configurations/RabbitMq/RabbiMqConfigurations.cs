using MassTransit;
using Message.Broker.RabbitMq;
using Messaging.Contracts.Events.Orders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Domain.Execeptions;
using Order.Infrastructure.Configurations.RabbitMq;
using Order.Infrastructure.Consumers;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;

namespace Product.Consumer.Configurations;

[ExcludeFromCodeCoverage]
public static class RabbiMqConfigurations
{
    public static IServiceCollection AddMassTransitSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMqTransportOptions"));

        var rabbitMqOptions = new RabbitMqConfig();
        configuration.GetSection("RabbitMqTransportOptions").Bind(rabbitMqOptions);

        services.AddMassTransit(x =>
        {

            x.AddConsumers(typeof(OrderConsumer).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {

                cfg.Host(rabbitMqOptions.Host, host =>
                {
                    host.Username(rabbitMqOptions.User);
                    host.Password(rabbitMqOptions.Pass);

                    if (rabbitMqOptions.UseSsl)
                        host.UseSsl(ssl => ssl.Protocol = SslProtocols.Tls12);
                });


                cfg.ConfigureEndpoint<OrderConsumer>(context,
                    GetRabbitEndpointConfig(nameof(OrderCreatedEvent), $"{rabbitMqOptions.Prefix}{QueueEndPointConfig.OrderCreatedMessage}"));

            });

        });

        return services;
    }

    static void ConfigureEndpoint<TConsumer>(this IRabbitMqBusFactoryConfigurator configRabbit,
        IBusRegistrationContext context,
        RabbitMqEndpointConfig endpointConfig)
        where TConsumer : class, IConsumer
    {
        configRabbit.ReceiveEndpoint(endpointConfig.QueueName!, configureEndpoint =>
        {
            configureEndpoint.ConfigureConsumeTopology = endpointConfig.ConfigureConsumeTopology;
            configureEndpoint.PrefetchCount = endpointConfig.PrefetchCount;
            configureEndpoint.UseMessageRetry(retry =>
            {
                retry.Interval(endpointConfig.RetryLimit, endpointConfig.Interval);
                retry.Ignore<ConsumerCanceledException>();
                retry.Exponential(3, TimeSpan.FromSeconds(5), TimeSpan.FromHours(1), TimeSpan.FromSeconds(10))
                .Handle<CasoftStoreRetryException>();
            });

            configureEndpoint.ConfigureConsumer<TConsumer>(context);

        });
    }

    private static RabbitMqEndpointConfig GetRabbitEndpointConfig(string routingKey, string queueName)
    {
        return new RabbitMqEndpointConfig
        {
            QueueName = queueName,
            RoutingKey = routingKey,
            ExchangeType = RabbitMQ.Client.ExchangeType.Fanout,
            RetryLimit = 2,
            Interval = TimeSpan.FromSeconds(3),
            ConfigureConsumeTopology = false,
            PrefetchCount = 5
        };
    }
}