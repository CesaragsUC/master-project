namespace Auth.Api.Abstractions;

public interface IAuthKeyCloakService : IAuthService
{
    Task<bool> SpecificKeyCloackMethod();
}
