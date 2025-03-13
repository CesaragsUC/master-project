using Billing.Api.Configurations;
using Billing.Application.Service;
using Billing.Infrastructure.Configurations;
using Shared.Kernel.CloudConfig;
using Shared.Kernel.Opentelemetry;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

builder.Services.AddControllers();

var environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? string.Empty;

builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddServices(builder.Configuration);
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
