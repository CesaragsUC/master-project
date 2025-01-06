using Azure.Storage.Blobs;
using Domain.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ResultNet;
using Product.Services.Configuration;
using Product.Domain.Configurations;
using Infrastructure.Repository;

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
        services.AddProductServices(configuration);

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

        var metaDataConfig = configuration.GetSection("Keycloak:MetadataAddress");

        services.AddKeycloakAuthentication(authenticationOptions!, options =>
        {
            options.MetadataAddress = metaDataConfig.Value!;
            options.RequireHttpsMetadata = false;
        });


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

        Console.WriteLine($"RabbitMQ Host: {host}, User: {user}");

        services.AddMassTransit(m =>
        {
            m.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                //message is removed from the queue and then redelivered to the queue at a future time.
                //Now, if the initial 5 immediate retries fail(the database is really, really down),
                //the message will retry an additional three times after 5, 15, and 30 minutes.
                //This could mean a total of 15 retry attempts(on top of the initial 4 attempts prior to the retry / redelivery filters taking control).
                cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));

                //attempts to deliver the message to a consumer five times before throwing the exception back up the pipeline.
                cfg.UseMessageRetry(r => r.Immediate(5));
            });

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
