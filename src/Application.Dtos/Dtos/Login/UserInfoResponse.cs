using System.Text.Json.Serialization;

namespace Application.Dtos.Dtos.Login;

/// <summary>
/// this class is used to bind with keycloak response
/// </summary>
public class UserInfoResponse
{
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }

    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }

    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool EmailVeried { get; set; }
}