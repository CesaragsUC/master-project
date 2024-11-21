using Application.Dtos.Dtos.Login;
using Application.Dtos.Settings;
using Auth.Api.Abstractions;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace Auth.Api.Services;

public class KeycloakAuthService : IAuthKeyCloakService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<KeycloakSettings> _keyCloakOptions;

    public KeycloakAuthService(HttpClient httpClient, IOptions<KeycloakSettings> keyCloakOptions)
    {
        _httpClient = httpClient;
        _keyCloakOptions = keyCloakOptions;
    }

    public async Task<TokenResponse> GetToken(string email, string password)
    {
        var keycloakUrl = $"{_keyCloakOptions.Value.AuthServerUrl}/token";
        var clientId = _keyCloakOptions.Value.Resource;
        var clientSecret = _keyCloakOptions.Value.Credentials?.Secret; 

        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId!),
            new KeyValuePair<string, string>("client_secret", clientSecret!),
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password)
        });

        var response = await _httpClient.PostAsync(keycloakUrl, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Failed to authenticate with Keycloak: {StatusCode}", response.StatusCode);
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(responseContent);
    }

    public async Task<bool> Logout(string refreshToken)
    {
        var keycloakLogoutUrl = $"{_keyCloakOptions.Value.AuthServerUrl}/logout";
        var clientId = _keyCloakOptions.Value.Resource;
        var clientSecret = _keyCloakOptions.Value.Credentials?.Secret;

        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId!),
            new KeyValuePair<string, string>("client_secret", clientSecret!),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        });

        var response = await _httpClient.PostAsync(keycloakLogoutUrl, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Failed to logout from Keycloak: {StatusCode}", response.StatusCode);
            return false;
        }

        return true;
    }

    public async Task<bool> SpecificKeyCloackMethod()
    {
        return true;
    }
}
