using Billing.Api.Configurations;
using Billing.Application.Service;
using Billing.Infrastructure;
using Billing.Infrastructure.Configurations;
using Billing.Infrastructure.RabbitMq;
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
builder.Services.AddServices(builder.Configuration);
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
