using Azure.Storage.Blobs;
using Domain.Configurations;
using Domain.Interfaces;
using Infrasctructure;
using Infrastructure.Configurations;
using Infrastructure.Repository;
using Infrastructure.Services;
using InfraStructure;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(cf =>
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

    var authenticationOptions = builder
                            .Configuration
                            .GetSection(KeycloakAuthenticationOptions.Section)
                            .Get<KeycloakAuthenticationOptions>();

    builder.Services.AddKeycloakAuthentication(authenticationOptions);


    var authorizationOptions = builder
                                .Configuration
                                .GetSection(KeycloakProtectionClientOptions.Section)
                                .Get<KeycloakProtectionClientOptions>();

    builder.Services.AddKeycloakAuthorization(authorizationOptions);


    builder.Services.AddMassTransit(m =>
    {
        m.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host("localhost", "/", c =>
            {
                c.Username("guest");
                c.Password("guest");
            });
            cfg.ConfigureEndpoints(ctx);
        });
    });

    builder.Services.AddMediatrService();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IBobStorageService, BobStorageService>();

    builder.Services.PostgresDbService(builder.Configuration);


    builder.Services.Configure<BlobContainers>(builder.Configuration.GetSection("BlobContainers"));

    builder.Services.AddSingleton(provider =>
    {
        var blobContainers = provider.GetRequiredService<IOptions<BlobContainers>>().Value;
        return new BlobServiceClient(blobContainers.ConnectionStrings);
    });

    ////opção 1 com classe de configuração
    //builder.Services.AddScoped<IMigratorService, MigratorService>();

    ////opção 2 com extensão
    builder.Services.ConfigureFluentMigration(builder.Configuration);

  builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService("Product.Api"))
  .WithTracing(builder =>
  {
      builder
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "localhost";
                options.AgentPort = 6831;
            });
  });


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Error(ex, "Erro ao iniciar a aplicação");
    throw;
}


