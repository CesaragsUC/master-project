using Basket.Api.Abstractions;
using Basket.Api.Configurations;
using Basket.Api.Services;
using Basket.Domain.Abstractions;
using Basket.Infrastructure.Configurations;
using Basket.Infrastructure.Repository;
using Shared.Kernel.Opentelemetry;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddServices(builder.Configuration);
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddMemoryCache();
builder.Services.AddInfra(builder.Configuration);

string connectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value!;

IConnectionMultiplexer connectionMultiplexer =
    ConnectionMultiplexer.Connect(connectionString);

builder.Services.AddSingleton(connectionMultiplexer);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConnectionMultiplexerFactory =
        () => Task.FromResult(connectionMultiplexer);
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

// more configuring metrics for grafana
app.UseOpenTelemetryPrometheusScrapingEndpoint();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
