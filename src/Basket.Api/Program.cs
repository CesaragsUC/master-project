using Basket.Api.Abstractions;
using Basket.Api.Configurations;
using Basket.Api.Services;
using Basket.Domain.Abstractions;
using Basket.Infrastructure.Configurations;
using Basket.Infrastructure.Repository;
using Shared.Kernel.CloudConfig;
using Shared.Kernel.Opentelemetry;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? string.Empty;

// LOAD CONFIGURATION FROM AZURE KEY VAULT BEFORE ANYTHING ELSE IN CASE RUNNING IN PRODUCTION ENVIRONMENT
// IF ITS RUNING IN DEVELOPMENT OR DOCKER ENVIRONMENT, THE CONFIGURATION WILL BE LOADED FROM appsettings.Docker.json or appsettings.Development.json
builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);

OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddMemoryCache();
builder.Services.AddInfra(builder.Configuration);
OpenTelemetrySetup.GrafanaOpenTelemetrySetup();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

// more configuring metrics for grafana
app.UseOpenTelemetryPrometheusScrapingEndpoint();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
