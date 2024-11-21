using Microsoft.Extensions.Configuration;

namespace Application.Dtos.Settings;

public class KeycloakSettings
{
    [ConfigurationKeyName("realm")]
    public string? Realm { get; set; }

    [ConfigurationKeyName("auth-server-url")]
    public string? AuthServerUrl { get; set; }

    [ConfigurationKeyName("ssl-required")]
    public string? SslRequired { get; set; }

    [ConfigurationKeyName("resource")]
    public string? Resource { get; set; }

    [ConfigurationKeyName("verify-token-audience")]
    public bool VerifyTokenAudience { get; set; }

    [ConfigurationKeyName("credentials")]
    public Credentials? Credentials { get; set; }

    [ConfigurationKeyName("use-resource-role-mappings")]
    public bool UseResourceRoleMappings { get; set; }

    [ConfigurationKeyName("confidential-port")]
    public int ConfidentialPort { get; set; }

    [ConfigurationKeyName("policy-enforcer")]
    public PolicyEnforcer? PolicyEnforcer { get; set; }
}

public class Credentials
{
    [ConfigurationKeyName("secret")]
    public string? Secret { get; set; }
}
public class PolicyEnforcer
{
    [ConfigurationKeyName("credentials")]
    public Dictionary<string, string>? Credentials { get; set; }
}