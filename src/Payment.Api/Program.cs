using Billing.Api.Configurations;
using Billing.Infrastructure;
using Billing.Infrastructure.Configurations;
using Billing.Infrastructure.Configurations.RabbitMq;
using Billing.Infrastructure.RabbitMq;
using Billing.Service;
using Billing.Service.Configurations;
using RepoPgNet;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureFluentMigration(builder.Configuration);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddRepoPgNet<BillingContext>(builder.Configuration);
builder.Services.AddRefitConfig(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddInfra(builder.Configuration);


var app = builder.Build();

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
