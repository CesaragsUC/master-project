using Auth.Api.Abstractions;
using Auth.Api.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using ResultNet;
using Shared.Kernel.Models;
using Shared.Kernel.Opentelemetry;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

OpenTelemetrySetup.SetupLogging(builder, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthKeyCloakService, KeycloakAuthService>();
builder.Services.AddScoped(typeof(IResult<>), typeof(Result<>));

builder.Services.AddGrafanaSetup(builder.Configuration);
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, KeycloakAuthenticationOptions.Section);

builder.Services.AddKeycloakAuthorization(builder.Configuration, KeycloakAuthenticationOptions.Section);

// bind the KeycloakSettings to the configuration
builder.Services.Configure<KeycloakSettings>(
    builder.Configuration.GetSection(KeycloakAuthenticationOptions.Section));


builder.Services.AddHttpClient();

var app = builder.Build();

// more configuring metrics for grafana
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.MapControllers();

app.Run();
