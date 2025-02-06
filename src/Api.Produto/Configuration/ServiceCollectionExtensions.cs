using Domain.Interfaces;
using Infrastructure.Services;
using Microsoft.OpenApi.Models;
using Product.Application.Configurations;
using Product.Application.Services;
using Product.Domain.Abstractions;
using ResultNet;

namespace Product.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IBobStorageService, BobStorageService>();
        services.AddScoped(typeof(IResult<>), typeof(Result<>));
        services.AddProductServices(configuration);
        services.AddSwaggerServices();
        services.AddApplicationServices(configuration);

    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {

        services.AddSwaggerGen(cf =>
        {
            cf.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Service", Version = "v1" });

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


    public static IServiceCollection AddProductServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
