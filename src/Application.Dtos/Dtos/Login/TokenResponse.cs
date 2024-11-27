using System.Text.Json.Serialization;

namespace Application.Dtos.Dtos.Login;

public record TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    public UserInfoResponse? User { get; set; }
}
