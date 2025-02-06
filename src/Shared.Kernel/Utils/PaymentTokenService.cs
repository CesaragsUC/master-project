using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Utils;

[ExcludeFromCodeCoverage]
public static class PaymentTokenService
{
    public static string GenerateToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static bool ValidateToken(string token)
    {
        return !string.IsNullOrEmpty(token) && token.Length == 32;
    }
}