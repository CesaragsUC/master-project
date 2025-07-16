using Infrastructure.Configurations;
using Product.Api.Configuration;
using Product.Api.Exceptions;
using Shared.Kernel.Opentelemetry;
using Serilog;
using Shared.Kernel.CloudConfig;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? string.Empty;

    // LOAD CONFIGURATION FROM AZURE KEY VAULT BEFORE ANYTHING ELSE IN CASE RUNNING IN PRODUCTION ENVIRONMENT
    // IF ITS RUNING IN DEVELOPMENT OR DOCKER ENVIRONMENT, THE CONFIGURATION WILL BE LOADED FROM appsettings.Docker.json or appsettings.Development.json
    builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

    builder.Services.AddServices(builder.Configuration);
    builder.Services.AddInfra(builder.Configuration);
    builder.Services.AddExceptionHandler<ProductInvalidExceptionHandler>();
    builder.Services.AddExceptionHandler<InvalidExceptionHandler>();
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