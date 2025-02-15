using Auth.Api.Abstractions;
using Auth.Api.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
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

builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService("Auth.Api"))
  .WithTracing(builder =>
  {
      builder
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "localhost";
                options.AgentPort = 6831;
            });
  });


var jaegerConfig = builder.Configuration.GetSection("OpenTelemetry");
var serviceName = jaegerConfig.GetValue<string>("ServiceName");
var jaegerHost = jaegerConfig.GetValue<string>("Jaeger:AgentHost");
var jaegerPort = jaegerConfig.GetValue<int>("Jaeger:AgentPort");


builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
              .AddJaegerExporter(options =>
              {
                  options.AgentHost = jaegerHost;
                  options.AgentPort = jaegerPort;
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
