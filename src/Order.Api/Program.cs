using Order.Api.Configurations;
using Order.Application.Abstractions;
using Order.Application.Service;
using Order.Infrastructure.Configurations;
using Shared.Kernel.CloudConfig;
using Shared.Kernel.Opentelemetry;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

var environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? string.Empty;

builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddServices();
builder.Services.AddInfra(builder.Configuration);


var app = builder.Build();

// more configuring metrics for grafana
app.UseOpenTelemetryPrometheusScrapingEndpoint();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
