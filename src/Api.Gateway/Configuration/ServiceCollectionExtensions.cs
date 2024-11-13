using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;


namespace Api.Gateway.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOceloConfigurations(this IServiceCollection services, ConfigurationManager configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"ocelot.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"ocelot.{environment}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
            return services;
        }
    }
}
