using Billing.Consumer.Execeptions;
using Billing.Infrastructure.Configurations.RabbitMq;
using Billing.Infrastructure.Consumers;
using MassTransit;
using Message.Broker.Configurations;
using Message.Broker.RabbitMq;
using Messaging.Contracts.Events.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;

namespace Billing.Infrastructure.RabbitMq;

[ExcludeFromCodeCoverage]
public static class RabbiMqConfigurations
{

    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMassTransitSetup(configuration);
        services.AddMassTransitFactory(configuration);
        return services;
    }

    public static IServiceCollection AddMassTransitSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMqTransportOptions"));

        var rabbitMqOptions = new RabbitMqConfig();
        configuration.GetSection("RabbitMqTransportOptions").Bind(rabbitMqOptions);

        services.AddMassTransit(x =>
        {

            x.AddConsumers(typeof(PaymentConsumer).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {

                cfg.Host(rabbitMqOptions.Host, host =>
                {
                    host.Username(rabbitMqOptions.User);
                    host.Password(rabbitMqOptions.Pass);

                    if (rabbitMqOptions.UseSsl)
                        host.UseSsl(ssl => ssl.Protocol = SslProtocols.Tls12);
                });


                cfg.ConfigureEndpoint<PaymentConsumer>(context,
                    GetRabbitEndpointConfig(nameof(PaymentCreatedEvent), $"{rabbitMqOptions.Prefix}{QueueEndPointConfig.PaymentCreatedMessage}"));

                cfg.ConfigureEndpoint<PaymentFailedConsumer>(context,
                    GetRabbitEndpointConfig(nameof(PaymentFailedEvent), $"{rabbitMqOptions.Prefix}{QueueEndPointConfig.PaymentFailedMessage}"));

                cfg.ConfigureEndpoint<PaymentConfirmedConsumer>(context,
                     GetRabbitEndpointConfig(nameof(PaymentConfirmedEvent), $"{rabbitMqOptions.Prefix}{QueueEndPointConfig.PaymentConfirmedMessage}"));

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