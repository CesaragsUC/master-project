using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Utils;

[ExcludeFromCodeCoverage]
public static class OrderHelper
{
    public static string GenerateOrderNumber(string prefix = "ORD")
    {
        return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
    }

}
