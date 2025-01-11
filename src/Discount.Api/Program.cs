using Discount.Api.Services;
using Discount.Domain.Abstractions;
using Discount.Infrastructure;
using Discount.Infrastructure.Configurations;
using RepoPgNet;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepoPgNet<CouponsDbContext>(builder.Configuration);
builder.Services.PostgresDbService(builder.Configuration);

builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

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
