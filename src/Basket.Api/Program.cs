using Basket.Api.Abstractions;
using Basket.Api.Services;
using Basket.Domain.Abstractions;
using Basket.Infrastructure.Configurations;
using Basket.Infrastructure.Repository;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddMemoryCache();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
