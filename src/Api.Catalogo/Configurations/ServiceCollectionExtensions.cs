using Api.Catalogo.DbContext;

namespace Api.Catalogo.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection MongoDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        return services;
    }
}
