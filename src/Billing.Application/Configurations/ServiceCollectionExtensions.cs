using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using Refit;
using Billing.Application.Abstractions;

namespace Billing.Application.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationService(this IServiceCollection services, IConfiguration configuration)
    {
        //Registra todos os handlers do MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddRefitConfig(configuration);
    }

    public static void AddRefitConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var baseDiscountUri = configuration.GetSection("OrderApi:BaseUrl").Value;

        services.AddRefitClient<IOderApi>(new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            })

        }).ConfigureHttpClient(c => c.BaseAddress = new Uri(baseDiscountUri!));

    }
}
