using EasyMongoNet.Exntesions;
using Message.Broker.Configurations;
using Message.Broker.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration);
        services.AddMassTransitFactory(configuration);
    }
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEasyMongoNet(configuration);
    }

}
