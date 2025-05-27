using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Utils;

[ExcludeFromCodeCoverage]
public static class EnvironmentCheck
{
    public static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
    public static bool IsProduction()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
    }
    public static bool IsStaging()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Staging";
    }
}
