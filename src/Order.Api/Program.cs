using Order.Api.Configurations;
using Order.Application.Service;
using Order.Infrastructure;
using Order.Infrastructure.Configurations;
using RepoPgNet;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRepoPgNet<OrderDbContext>(builder.Configuration);
builder.Services.ConfigureFluentMigration(builder.Configuration);
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
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
