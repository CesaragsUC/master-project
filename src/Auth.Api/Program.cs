using Application.Dtos.Abstractions;
using Application.Dtos.Dtos.Response;
using Application.Dtos.Settings;
using Auth.Api.Abstractions;
using Auth.Api.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
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



builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, KeycloakAuthenticationOptions.Section);

builder.Services.AddKeycloakAuthorization(builder.Configuration, KeycloakAuthenticationOptions.Section);

// bind the KeycloakSettings to the configuration
builder.Services.Configure<KeycloakSettings>(
    builder.Configuration.GetSection(KeycloakAuthenticationOptions.Section));


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
