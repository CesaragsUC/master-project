using Auth.Api.Abstractions;
using Auth.Api.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using ResultNet;
using Serilog;
using Shared.Kernel.Models;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthKeyCloakService, KeycloakAuthService>();
builder.Services.AddScoped(typeof(IResult<>), typeof(Result<>));

//https://medium.com/@jwag/grafana-with-logging-and-metrics-from-a-dotnet-api-cbaa06d359aa
//https://github.com/sgbj/dotnet-monitoring
//https://github.com/jaaywags/grafana-dotnet-demo

var openTelemetryOptions = new OpenTelemetryOptions();
builder.Configuration.GetSection("OpenTelemetryOptions").Bind(openTelemetryOptions);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(openTelemetryOptions.AppName))
    .WithTracing(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
              .AddJaegerExporter(options =>
              {
                  options.AgentHost = openTelemetryOptions.Jaeger.AgentHost;
                  options.AgentPort = openTelemetryOptions.Jaeger.AgentPort;
              });
    });

builder.Services.UseHttpClientMetrics();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, KeycloakAuthenticationOptions.Section);

builder.Services.AddKeycloakAuthorization(builder.Configuration, KeycloakAuthenticationOptions.Section);

// bind the KeycloakSettings to the configuration
builder.Services.Configure<KeycloakSettings>(
    builder.Configuration.GetSection(KeycloakAuthenticationOptions.Section));


builder.Services.AddHttpClient();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.MapControllers();

app.Run();
