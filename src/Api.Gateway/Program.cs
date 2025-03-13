using Api.Gateway.Configuration;
using Api.Gateway.Dto;
using Api.Gateway.Dtos;
using Api.Gateway.Services;
using Microsoft.OpenApi.Models;
using Ocelot.Middleware;
using Serilog;
using Shared.Kernel.CloudConfig;
using Shared.Kernel.Opentelemetry;

try
{
    var builder = WebApplication.CreateBuilder(args);

    OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

    var environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? string.Empty;

    builder.Services.AzureKeyVaultConfig(builder, builder.Configuration, environment);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerForOcelot(builder.Configuration, (o) =>
    {
        o.GenerateDocsForGatewayItSelf = true;
    });

    builder.Services.AddSwaggerGen(cf =>
    {
        cf.SwaggerDoc("v1", new OpenApiInfo { Title = "Gateway", Version = "v1" });
    });

    builder.Services.AddOceloConfigurations(builder.Configuration);
    builder.Services.AddServices(builder.Configuration);

    var _fontUri = builder.Configuration.GetSection("FrontEndUri").Get<FrontEndUri>();

    var app = builder.Build();

    // more configuring metrics for grafana
    app.UseOpenTelemetryPrometheusScrapingEndpoint();


    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerForOcelotUI();



    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseRouting();

    app.UseCors(_fontUri?.Name!);

    app.MapControllers();
    app.UseAuthorization();

#pragma warning disable ASP0014
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
#pragma warning restore ASP0014

    // call the authentication service
    app.MapPost("/api/masterauth/login", async (LoginDto loginDto, AuthenticationService authenticationService) =>
    {
        var result = await authenticationService.Authenticate(loginDto);
        return Results.Ok(result);
    });

    // call the logout service
    app.MapPost("/api/masterauth/logout", async (string refreshToken, AuthenticationService authenticationService) =>
    {
        var result = await authenticationService.Logout(refreshToken);
        return Results.Ok(result);
    });



    app.UseOcelot().Wait();
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