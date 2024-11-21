namespace Auth.Api.Abstractions;

public interface IIdentityServerAuthService : IAuthService
{
    Task<bool> SpecificIdentityServerAuthMethod();
}
