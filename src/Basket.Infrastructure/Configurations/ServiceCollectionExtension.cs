using EasyMongoNet.Exntesions;
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
    }
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEasyMongoNet(configuration);
        services.AddMessageBrokerSetup(configuration);
    }

}
