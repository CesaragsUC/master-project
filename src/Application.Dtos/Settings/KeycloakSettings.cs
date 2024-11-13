namespace Application.Dtos.Settings;

public class KeycloakSettings
{
    public string Realm { get; set; }
    public string AuthServerUrl { get; set; }
    public string SslRequired { get; set; }
    public string Resource { get; set; }
    public bool VerifyTokenAudience { get; set; }
    public Credentials Credentials { get; set; }
    public bool UseResourceRoleMappings { get; set; }
    public int ConfidentialPort { get; set; }
    public PolicyEnforcer PolicyEnforcer { get; set; }
}

public class Credentials
{
    public string Secret { get; set; }
}
public class PolicyEnforcer
{
    public Dictionary<string, string> Credentials { get; set; }
}