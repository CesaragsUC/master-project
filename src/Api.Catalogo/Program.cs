using Catalog.Domain.Abstractions;
using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Repository;
using Catalog.Service.Abstractions;
using Catalog.Service.Services;
using Catalog.Services.Abstractions;
using Catalog.Services.Filters;
using Catalogo.Api.Configurations;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;

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
        cf.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog Api", Version = "v1" });

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

    builder.Services.AddKeycloakAuthentication(authenticationOptions!);


    var authorizationOptions = builder
                                .Configuration
                                .GetSection(KeycloakProtectionClientOptions.Section)
                                .Get<KeycloakProtectionClientOptions>();

    builder.Services.AddKeycloakAuthorization(authorizationOptions!);



    builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();


    builder.Services.AddScoped<IQueryFilter, ProductFilter>();

    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

    builder.Services.MongoDbService(builder.Configuration);

    builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService("Catalog.Api"))
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
        app.UseSwaggerUI(c =>
        {
            c.OAuthClientId("casoft-system"); // ID do cliente configurado no Keycloak
            c.OAuthAppName("Catalog API Swagger");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}