using Basket.Api.Abstractions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Refit;
using MassTransit;

namespace Basket.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfraConfig(this IServiceCollection services, IConfiguration configuration)
    {
        AddRefitConfig(services, configuration);
        AddMassTransientServices(services, configuration);

        return services;
    }

    public static void AddRefitConfig(IServiceCollection services, IConfiguration configuration)
    {
        var baseDiscountUri = configuration.GetSection("DiscountApi:BaseUrl").Value;

        services.AddRefitClient<IDiscountApi>(new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            })

        }).ConfigureHttpClient(c => c.BaseAddress = new Uri(baseDiscountUri!));

    }

    public static void AddMassTransientServices(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmqConfig = configuration.GetSection("RabbitMqTransport");
        var host = rabbitmqConfig.GetValue<string>("Host");
        var user = rabbitmqConfig.GetValue<string>("User");
        var pass = rabbitmqConfig.GetValue<string>("Pass");

        Console.WriteLine($"RabbitMQ Host: {host}, User: {user}");

        services.AddMassTransit(m =>
        {
            m.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                //message is removed from the queue and then redelivered to the queue at a future time.
                //Now, if the initial 5 immediate retries fail(the database is really, really down),
                //the message will retry an additional three times after 5, 15, and 30 minutes.
                //This could mean a total of 15 retry attempts(on top of the initial 4 attempts prior to the retry / redelivery filters taking control).
                cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));

                //attempts to deliver the message to a consumer five times before throwing the exception back up the pipeline.
                cfg.UseMessageRetry(r => r.Immediate(5));
            });

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
