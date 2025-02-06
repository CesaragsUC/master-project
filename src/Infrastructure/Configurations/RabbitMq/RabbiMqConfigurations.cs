using MassTransit;
using Message.Broker.RabbitMq;
using Message.Broker.RabbitMq.Configurations;
using Messaging.Contracts.Events.Product;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Domain.Exceptions;
using Product.Infrastructure.Consumers;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;

namespace Product.Infrastructure.RabbitMq;

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

            x.AddConsumers(typeof(ProductAddedConsumer).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {

                cfg.Host(rabbitMqOptions.Host, host =>
                {
                    host.Username(rabbitMqOptions.User);
                    host.Password(rabbitMqOptions.Pass);

                    if (rabbitMqOptions.UseSsl)
                        host.UseSsl(ssl => ssl.Protocol = SslProtocols.Tls12);
                   
                    
                    host.PublisherConfirmation = true;
                });


                cfg.ConfigureEndpoint<ProductAddedConsumer>(context,
                    GetRabbitEndpointConfig(nameof(ProductAddedEvent), $"{rabbitMqOptions.Prefix}{QueueConfig.ProductCreatedMessage}"));

                cfg.ConfigureEndpoint<ProdutctDeletedConsumer>(context,
                    GetRabbitEndpointConfig(nameof(ProductDeletedEvent), $"{rabbitMqOptions.Prefix}{QueueConfig.ProductDeletedMessage}"));

                cfg.ConfigureEndpoint<ProdutctUpdatedConsumer>(context,
                    GetRabbitEndpointConfig(nameof(ProductUpdatedEvent), $"{rabbitMqOptions.Prefix}{QueueConfig.ProductUpdatedMessage}"));

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