namespace Shared.Kernel.Utils;

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