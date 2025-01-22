using EasyMongoNet.Exntesions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Product.Consumer.Configurations;


//https://github.com/quartznet/quartznet/tree/main/database/tables

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.MassTransitServices();
        services.MongoDbService();
        services.AddMediatrService();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection MassTransitServices(this IServiceCollection services)
    { 

        var configuration = GetConfigBuilder().Build();

        services.Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMqTransport"));

        services.AddMassTransit(x =>
        {
            x.AddPublishMessageScheduler();

            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UsePublishMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
        });


        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = true;
        });

        return services;
    }

    public static IServiceCollection MongoDbService(this IServiceCollection services)
    {
        var configuration = GetConfigBuilder().Build();

        services.AddEasyMongoNet(configuration);

        return services;
    }

    public static void AddMediatrService(this IServiceCollection services)
    {
        //Registra todos os handlers do MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
    }

    private static IConfigurationBuilder GetConfigBuilder()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .AddEnvironmentVariables();

        return builder;
    }
}
