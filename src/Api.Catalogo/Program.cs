
using Catalog.Domain.Abstractions;
using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Repository;
using Catalog.Service.Abstractions;
using Catalog.Service.Services;
using Catalog.Services.Abstractions;
using Catalog.Services.Filters;
using Catalogo.Api.Configurations;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(cf => {
        cf.SwaggerDoc("v1", new OpenApiInfo { Title = "Produto Catalogo", Version = "v1" });
        cf.SchemaFilter<SwaggerSchemaFilterConfig>();
    });

    builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();
    
   
    builder.Services.AddScoped<IQueryFilter, ProductFilter>();

    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

    builder.Services.MongoDbService(builder.Configuration);

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

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}