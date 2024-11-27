using Application.Dtos.Dtos.Login;
using Application.Dtos.Dtos.Response;
using Application.Dtos.Settings;
using Auth.Api.Abstractions;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Http.Headers;
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

    public async Task<Result<TokenResponse>> GetToken(string email, string password)
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
            return await Result<TokenResponse>.FailureAsync(400, "An error occour during login");

        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result =  JsonSerializer.Deserialize<TokenResponse>(responseContent);

        var userInfo = await GetUserInfo(result!.AccessToken!);

        var infoResponse = new UserInfoResponse {

            Name = userInfo.Name,
            FamilyName = userInfo.FamilyName,
            GivenName = userInfo.GivenName,
            Email = userInfo.Email,
            EmailVeried = userInfo.EmailVeried
        };

        result.User = infoResponse;

        return await Result<TokenResponse>.SuccessAsync(result!);
    }


    public async Task<UserInfoResponse> GetUserInfo(string accessToken)
    {
        var keycloakUrl = $"{_keyCloakOptions.Value.AuthServerUrl}/userinfo";

        // Configurar a requisição com o token de acesso
        var request = new HttpRequestMessage(HttpMethod.Get, keycloakUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Enviar requisição
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Failed to fetch user info: {Message}",response.StatusCode);

            return null!;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return  JsonSerializer.Deserialize<UserInfoResponse>(responseContent);
    }

    public async Task<Result<bool>> Logout(string refreshToken)
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
            return await Result<bool>.FailureAsync(500, "An error occour during login");
        }

        return await Result<bool>.SuccessAsync("logout success.");
    }

}
