using Azure.Storage.Blobs;
using Catalog.Application.Abstractions;
using Catalog.Application.Services;
using Catalog.Infrastructure.Configurations;
using Catalog.Infrastructure.Repository;
using EasyMongoNet.Exntesions;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Shared.Kernel.Opentelemetry;
using ResultNet;
using System.Reflection;
using Catalog.Domain.Abstractions;
using Shared.Kernel.KeyCloackConfig;

namespace Catalogo.Api.Configurations;

public static class ServiceCollectionExtensions
{

    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped(typeof(IResult<>), typeof(Result<>));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.MongoDbService(configuration);
        services.AddGrafanaSetup(configuration);
        services.AddSwaggerServices();
        services.AddKeycloakServices(configuration);
        services.AddAzureBlobServices(configuration);
    }


    public static IServiceCollection MongoDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEasyMongoNet(configuration, (int)HealthCheckOptions.Active);

        return services;
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {

        services.AddSwaggerGen(cf =>
        {
            cf.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Catalog", Version = "v1" });

            // Configuração do esquema de segurança JWT para o Swagger
            cf.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http, // Altere para Http para indicar que é um esquema de autenticação HTTP com Bearer
                Scheme = "bearer", // Especifique "bearer" para indicar o formato JWT
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Por favor, insira o token JWT no campo 'Authorization' usando o prefixo 'Bearer '"
            });

            cf.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

    }

    public static void AddAzureBlobServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobContainers>(configuration.GetSection("BlobContainers"));

        services.AddSingleton(provider =>
        {
            var blobContainers = provider.GetRequiredService<IOptions<BlobContainers>>().Value;
            return new BlobServiceClient(blobContainers.ConnectionStrings);
        });

    }
}
