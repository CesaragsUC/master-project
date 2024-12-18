﻿using Azure.Storage.Blobs;
using Domain.Interfaces;
using Infrasctructure;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Domain.Configurations;
using ResultNet;

namespace Product.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddMediatrService();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IBobStorageService, BobStorageService>();
        services.AddScoped(typeof(IResult<>), typeof(Result<>));
        services.PostgresDbService(configuration);

        ////opção 1 com classe de configuração
        //builder.Services.AddScoped<IMigratorService, MigratorService>();

        ////opção 2 com extensão
        services.ConfigureFluentMigration(configuration);

        services.AddSwaggerServices();
        services.AddKeycloakServices(configuration);
        services.AddOpenTelemetryServices(configuration);
        services.AddAzureBlobServices(configuration);
        services.AddMassTransientServices(configuration);
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


    public static void AddAzureBlobServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobContainers>(configuration.GetSection("BlobContainers"));

        services.AddSingleton(provider =>
        {
            var blobContainers = provider.GetRequiredService<IOptions<BlobContainers>>().Value;
            return new BlobServiceClient(blobContainers.ConnectionStrings);
        });

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

    public static void AddMassTransientServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmqConfig = configuration.GetSection("RabbitMqTransport");
        var host = rabbitmqConfig.GetValue<string>("Host");
        var user = rabbitmqConfig.GetValue<string>("User");
        var pass = rabbitmqConfig.GetValue<string>("Pass");

        services.AddMassTransit(m =>
        {
            m.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(host, "/", c =>
                {
                    c.Username(user);
                    c.Password(pass);
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });
    }
}
