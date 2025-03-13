using Catalogo.Api.Configurations;
using Serilog;
using Shared.Kernel.CloudConfig;
using Shared.Kernel.Opentelemetry;


try
{

    var builder = WebApplication.CreateBuilder(args);

    OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    var environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? string.Empty;

    builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);

    builder.Services.AddServices(builder.Configuration);


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