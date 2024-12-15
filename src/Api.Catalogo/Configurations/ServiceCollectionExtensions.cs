using Application.Dtos.Abstractions;
using Application.Dtos.Dtos.Response;
using Azure.Storage.Blobs;
using Catalog.Domain.Abstractions;
using Catalog.Infrastructure.Configurations;
using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Repository;
using Catalog.Service.Abstractions;
using Catalog.Service.Services;
using Catalog.Services.Abstractions;
using Catalog.Services.Filters;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace Catalogo.Api.Configurations;

public static class ServiceCollectionExtensions
{

    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IMongoDbContext, MongoDbContext>();
        services.AddScoped<IQueryFilter, ProductFilter>();
        services.AddScoped(typeof(IResult<>), typeof(Result<>));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.MongoDbService(configuration);


        services.AddSwaggerServices();
        services.AddKeycloakServices(configuration);
        services.AddOpenTelemetryServices(configuration);
        services.AddAzureBlobServices(configuration);
    }

    public static IServiceCollection MongoDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

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

    public static void AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationOptions = configuration
                    .GetSection(KeycloakAuthenticationOptions.Section)
                    .Get<KeycloakAuthenticationOptions>();

        services.AddKeycloakAuthentication(authenticationOptions!);


        var authorizationOptions = configuration
                                    .GetSection(KeycloakProtectionClientOptions.Section)
                                    .Get<KeycloakProtectionClientOptions>();

        services.AddKeycloakAuthorization(authorizationOptions!);
    }

    public static void AddOpenTelemetryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jaegerConfig = configuration.GetSection("OpenTelemetry");
        var serviceName = jaegerConfig.GetValue<string>("ServiceName");
        var jaegerHost = jaegerConfig.GetValue<string>("Jaeger:AgentHost");
        var jaegerPort = jaegerConfig.GetValue<int>("Jaeger:AgentPort");


        services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(serviceName))
        .WithTracing(builder =>
        {
            builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                  .AddJaegerExporter(options =>
                  {
                      options.AgentHost = jaegerHost;
                      options.AgentPort = jaegerPort;
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
