using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Kernel.KeyCloackConfig;

public static class KeycloaclConfiguration
{
    public static void AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationOptions = configuration
                    .GetSection(KeycloakAuthenticationOptions.Section)
                    .Get<KeycloakAuthenticationOptions>();

        var metadataConfig = configuration.GetSection("Keycloak:MetadataAddress");
        var requireHttpsMetadataOption = configuration.GetSection("Keycloak:Authentication:Schemes:Bearer:RequireHttpsMetadata");

        services.AddKeycloakAuthentication(authenticationOptions!, options =>
        {
            options.MetadataAddress = metadataConfig.Value!;
            options.RequireHttpsMetadata = bool.Parse(requireHttpsMetadataOption?.Value!);
        });


        var authorizationOptions = configuration
                                    .GetSection(KeycloakProtectionClientOptions.Section)
                                    .Get<KeycloakProtectionClientOptions>();

        services.AddKeycloakAuthorization(authorizationOptions!);

    }
}
