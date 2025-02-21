using Billing.Api.Configurations;
using Billing.Application.Service;
using Billing.Infrastructure.Configurations;
using Billing.Infrastructure.RabbitMq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

LogConfig.SetupLogging(builder, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureFluentMigration(builder.Configuration);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddInfra(builder.Configuration);


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
