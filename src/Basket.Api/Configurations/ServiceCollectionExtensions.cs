using Basket.Api.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace Basket.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddRefitConfig(services, configuration);
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
}
