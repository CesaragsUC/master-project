﻿using MassTransit;
using Message.Broker.Abstractions;
using Message.Broker.RabbitMq;
using Message.Broker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;

namespace Message.Broker.Configurations;

[ExcludeFromCodeCoverage]
public static class RabbitMqServiceExtensions
{

    public static IServiceCollection AddMassTransitSetupConfig(
        this IServiceCollection services,
        IConfiguration configuration,
        params Type[] consumerAssemblies)
    {
        services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMqTransportOptions"));

        var rabbitMqOptions = new RabbitMqConfig();
        configuration.GetSection("RabbitMqTransportOptions").Bind(rabbitMqOptions);

        services.AddSingleton<IRabbitMqService, RabbitMqService>();
        services.AddHostedService<RabbitMqHostedService>();
        services.AddMassTransit(x =>
        {
            foreach (var assembly in consumerAssemblies)
            {
                x.AddConsumers(assembly.Assembly);
            }

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqOptions.Host, host =>
                {
                    host.Username(rabbitMqOptions.User);
                    host.Password(rabbitMqOptions.Pass);

                    if (rabbitMqOptions.UseSsl)
                        host.UseSsl(ssl => ssl.Protocol = SslProtocols.Tls12);
                });

                foreach (var consumerType in consumerAssemblies)
                {
                    var queueConfig = GetRabbitEndpointConfig(consumerType, rabbitMqOptions);

                    var method = typeof(RabbitMqServiceExtensions)
                        .GetMethod(nameof(ConfigureEndpoint),
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Static)
                        ?.MakeGenericMethod(consumerType);

                    method?.Invoke(null, new object[] { cfg, context, queueConfig });
                }
            });
        });

        return services;
    }

    private static void ConfigureEndpoint<TConsumer>(
    this IRabbitMqBusFactoryConfigurator configRabbit,
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
                    .Handle<Exception>();
            });

            configureEndpoint.ConfigureConsumer<TConsumer>(context);
        });
    }

    private static RabbitMqEndpointConfig GetRabbitEndpointConfig(Type consumerType, RabbitMqConfig rabbitMqConfig)
    {
        var eventName = consumerType.Name.Replace("Consumer", ".Event");

        return new RabbitMqEndpointConfig
        {
            QueueName = $"{rabbitMqConfig.Prefix}.casoft.{eventName.ToLower()}.v1",
            RoutingKey = eventName,
            ExchangeType = RabbitMQ.Client.ExchangeType.Fanout,
            RetryLimit = 2,
            Interval = TimeSpan.FromSeconds(3),
            ConfigureConsumeTopology = false,
            PrefetchCount = 5
        };
    }
}
