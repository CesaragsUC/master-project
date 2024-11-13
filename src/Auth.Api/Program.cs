using Application.Dtos.Settings;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

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



builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, KeycloakAuthenticationOptions.Section);

builder.Services.AddKeycloakAuthorization(builder.Configuration, KeycloakAuthenticationOptions.Section);

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.MapControllers();


app.Run();
