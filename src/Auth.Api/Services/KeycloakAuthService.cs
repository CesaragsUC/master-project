using Application.Dtos.Dtos.Login;
using Application.Dtos.Dtos.Response;
using Application.Dtos.Settings;
using Auth.Api.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Auth.Api.Services;

public class KeycloakAuthService : IAuthKeyCloakService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<KeycloakSettings> _keyCloakOptions;

    private List<string> _roles;

    public KeycloakAuthService(HttpClient httpClient, IOptions<KeycloakSettings> keyCloakOptions)
    {
        _httpClient = httpClient;
        _keyCloakOptions = keyCloakOptions;
        _roles = new List<string>();
    }

    public async Task<Result<LoginResponse>> GetToken(string email, string password)
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
            return await Result<LoginResponse>.FailureAsync(400, "An error occour during login");

        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse =  JsonSerializer.Deserialize<TokenResponse>(responseContent);

        var userInfo = await GetUserInfo(tokenResponse!.AccessToken!);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenResponse.AccessToken);

        SetRoles(token);

        var loginResponse = await GetLoginResponse(token, userInfo, tokenResponse);

        return await Result<LoginResponse>.SuccessAsync(loginResponse!);
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

    private async Task<LoginResponse> GetLoginResponse(JwtSecurityToken? token,
        UserInfoResponse userInfo, 
        TokenResponse tokenResponse)
    {
        var groups = token?.Claims
            .Where(c => c.Type == "group")
            .Select(c => c.Value)
            .ToList();

        var userToken = new UserToken
        {
            Id = userInfo.Sub ?? Guid.NewGuid().ToString(),
            Email = userInfo.Email,
            Name = userInfo.Name,
            Claims = token?.Claims.Select(c => new Claims { Type = c.Type, Value = c.Value }),
            Groups = groups,
            Roles = _roles
        };

        var loginResponse = new LoginResponse
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
            IsAdmin =  _roles.Select(c=> c.ToLower()).ToList().Contains("admin"),
            UserToken = userToken,
            Name = userInfo.Name,
            Email = userInfo.Email

        };


        return loginResponse;
    }
    private void SetRoles(JwtSecurityToken? token)
    {

        // realm_access
        var realmAccessClaim = token?.Claims.FirstOrDefault(c => c.Type == "realm_access");
        if (realmAccessClaim != null)
        {
            var realmRoles = JsonSerializer.Deserialize<RealmAccess>(realmAccessClaim.Value);
            if (realmRoles?.Roles != null)
            {
                _roles.AddRange(realmRoles.Roles);
            }
        }

        // resource_access
        var resourceAccessClaim = token?.Claims.FirstOrDefault(c => c.Type == "resource_access");
        if (resourceAccessClaim != null)
        {
            var resourceRoles = JsonSerializer.Deserialize<Dictionary<string, ResourceAccess>>(resourceAccessClaim.Value);
            if (resourceRoles != null)
            {
                foreach (var resource in resourceRoles?.Values)
                {
                    if (resource.Roles != null)
                    {
                        _roles.AddRange(resource.Roles);
                    }
                }
            }
        }
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
