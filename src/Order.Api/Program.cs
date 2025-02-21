using Order.Api.Configurations;
using Order.Application.Abstractions;
using Order.Application.Service;
using Order.Infrastructure.Configurations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

LogConfig.SetupLogging(builder, builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureFluentMigration(builder.Configuration);
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddServices();
builder.Services.AddInfra(builder.Configuration);


var app = builder.Build();

// more configuring metrics for grafana
app.UseOpenTelemetryPrometheusScrapingEndpoint();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
