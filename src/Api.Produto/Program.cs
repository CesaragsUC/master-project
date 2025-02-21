using Infrastructure.Configurations;
using Product.Api.Configuration;
using Product.Api.Exceptions;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    LogConfig.SetupLogging(builder, builder.Configuration);

    builder.Services.AddServices(builder.Configuration);
    builder.Services.AddInfra(builder.Configuration);

    builder.Services.AddExceptionHandler<ProductInvalidExceptionHandler>();
    builder.Services.AddExceptionHandler<ProductNotFoundExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // more configuring metrics for grafana
    app.UseOpenTelemetryPrometheusScrapingEndpoint();

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