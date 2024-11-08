
using Catalog.Infrastructure.Configurations;
using Catalog.Infrastructure.Context;

namespace Catalogo.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection MongoDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        return services;
    }
}
