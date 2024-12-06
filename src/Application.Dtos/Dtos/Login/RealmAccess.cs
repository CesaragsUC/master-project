using System.Text.Json.Serialization;

namespace Application.Dtos.Dtos.Login;

public class RealmAccess
{
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; }
}
