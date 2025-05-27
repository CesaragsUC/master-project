using Catalogo.Api.Configurations;
using Serilog;
using Shared.Kernel.CloudConfig;
using Shared.Kernel.Opentelemetry;


try
{

    var builder = WebApplication.CreateBuilder(args);

    var environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? string.Empty;

    // LOAD CONFIGURATION FROM AZURE KEY VAULT BEFORE ANYTHING ELSE IN CASE RUNNING IN PRODUCTION ENVIRONMENT
    // IF ITS RUNING IN DEVELOPMENT OR DOCKER ENVIRONMENT, THE CONFIGURATION WILL BE LOADED FROM appsettings.Docker.json or appsettings.Development.json
    builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);

    OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();


    builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);

    builder.Services.AddServices(builder.Configuration);
    OpenTelemetrySetup.GrafanaOpenTelemetrySetup();

    var app = builder.Build();

    // more configuring metrics for grafana
    app.UseOpenTelemetryPrometheusScrapingEndpoint();

    app.UseSwagger();
    app.UseSwaggerUI();


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