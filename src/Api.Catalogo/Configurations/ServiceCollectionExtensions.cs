
using Application.Dtos.Settings;
using Catalog.Infrastructure.Configurations;
using Catalog.Infrastructure.Context;
using Microsoft.OpenApi.Models;

namespace Catalogo.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection MongoDbService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        return services;
    }

    //public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    //{
    //    var keycloakSettings = new KeycloakSettings();
    //    configuration.GetSection("keycloak").Bind(keycloakSettings);
        
    //    services.AddSwaggerGen(c =>
    //    {
    //        var securityScheme = new OpenApiSecurityScheme
    //        {
    //            Name = "Keycloak",
    //            In = ParameterLocation.Header,
    //            Type = SecuritySchemeType.OpenIdConnect,
    //            OpenIdConnectUrl = new Uri($"{keycloakSettings.AuthServerUrl}realms/{keycloakSettings.Realm}/.well-known/openid-configuration"),
    //            Scheme = "bearer",
    //            BearerFormat = "JWT",
    //            Reference = new OpenApiReference
    //            {
    //                Id = "Bearer",
    //                Type = ReferenceType.SecurityScheme,
    //            }
    //        };
    //        c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    //        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //        {
    //            { securityScheme, Array.Empty<string>() }
    //        });
    //    });


    //    return services;
    //}
}
