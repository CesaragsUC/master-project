namespace Auth.Api.Abstractions;

public interface IAzureAdService : IAuthService
{
    Task<bool> SpecificAzureAdMethod();
}
