using Product.Api.Configuration;
using Product.Api.Exceptions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();


    builder.Services.AddServices(builder.Configuration);

    builder.Services.AddExceptionHandler<ProductInvalidExceptionHandler>();
    builder.Services.AddExceptionHandler<ProductNotFoundExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
        

    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


    app.UseHttpsRedirection();

    // Should be put before UseAuthorization, UseRouting and MapControllers
    app.UseExceptionHandler();

    app.UseAuthorization();

    app.MapControllers();


    app.Run();

}
catch (Exception ex)
{
    Log.Error(ex, "Erro ao iniciar a aplicacao");
    throw;
}


