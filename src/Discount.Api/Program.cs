using Discount.Api.Services;
using Discount.Domain.Abstractions;
using Discount.Infrastructure.Configurations;
using Shared.Kernel.Opentelemetry;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.PostgresDbService(builder.Configuration);

builder.Services.AddScoped<ICouponService, CouponService>();

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
