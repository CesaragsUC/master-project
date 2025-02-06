using Billing.Service.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Billing.Service.Configurations;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtentions
{
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

    public static void AddMediatrService(this IServiceCollection services)
    {
        //Registra todos os handlers do MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });


    }
}
