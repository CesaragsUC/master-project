using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoRepoNet;

namespace Basket.Infrastructure.Configurations;

public static class ServiceCollectionExtension
{
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoRepoNet(configuration);
    }
}
