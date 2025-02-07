using System.Diagnostics.CodeAnalysis;

namespace Billing.Domain.ValueObjects;


[ExcludeFromCodeCoverage]
public class DevsPayService
{
    public readonly string ApiKey;
    public readonly string EncryptionKey;

    public DevsPayService(string apiKey, string encryptionKey)
    {
        ApiKey = apiKey;
        EncryptionKey = encryptionKey;
    }
}
