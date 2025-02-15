using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Auth.Api.Dtos.Login;

[ExcludeFromCodeCoverage]
public class RealmAccess
{
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; }
}
