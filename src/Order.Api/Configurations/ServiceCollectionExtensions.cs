﻿using Microsoft.OpenApi.Models;
using Order.Application.Configurations;
using Order.Order.Application.Abstractions;
using Order.Order.Application.Services;

namespace Order.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddScoped<IQueueService, QueueService>();
        services.AddSwaggerServices();
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {

        services.AddSwaggerGen(cf =>
        {
            cf.SwaggerDoc("v1", new OpenApiInfo { Title = "Casoft Order Api", Version = "v1" });

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
}
