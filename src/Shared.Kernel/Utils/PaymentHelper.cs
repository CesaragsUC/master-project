using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Utils;

[ExcludeFromCodeCoverage]
public static class PaymentHelper
{
    public static string GenerateTransactionIdWithPrefix(string prefix = "PAY")
    {
        return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
    }

    public static string GenerateTransactionId()
    {
        return $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
    }
}
