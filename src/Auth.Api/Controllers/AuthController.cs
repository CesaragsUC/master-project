using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Auth.Api.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly HttpClient _httpClient;

    public AuthController(ILogger<AuthController> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var tokenResponse = await GetTokenFromKeycloak(loginRequest.Email, loginRequest.Password);

        if (tokenResponse == null)
        {
            return Unauthorized("Invalid credentials");
        }

        return Ok(tokenResponse);
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout(string refreshToken)
    {
        await LogoutFromKeycloak(refreshToken);
        return Ok("Logout Successful");
    }

    private async Task<TokenResponse> GetTokenFromKeycloak(string email, string password)
    {
        var keycloakUrl = "http://localhost:8180/realms/casoft/protocol/openid-connect/token";
        var clientId = "casoft-system";
        var clientSecret = "diXgtiln2QHvst3xwKNwQqVirDmWPD5x";

        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password)
        });

        var response = await _httpClient.PostAsync(keycloakUrl, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to authenticate with Keycloak: {StatusCode}", response.StatusCode);
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(responseContent);
    }

    private async Task<bool> LogoutFromKeycloak(string refreshToken)
    {
        var keycloakLogoutUrl = "http://localhost:8180/realms/casoft/protocol/openid-connect/logout";
        var clientId = "casoft-system";
        var clientSecret = "diXgtiln2QHvst3xwKNwQqVirDmWPD5x";

        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        });

        var response = await _httpClient.PostAsync(keycloakLogoutUrl, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to logout from Keycloak: {StatusCode}", response.StatusCode);
            return false;
        }

        return true;
    }

    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

}
