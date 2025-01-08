using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoRepoNet;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoRepoNet(configuration);
    }
}
