using System.Diagnostics.CodeAnalysis;

namespace Billing.Consumer.Execeptions;

[ExcludeFromCodeCoverage]
public class CasoftStoreRetryException : Exception
{
    public CasoftStoreRetryException()
    {
    }
    public CasoftStoreRetryException(string message) : base(message)
    {
    }
    public CasoftStoreRetryException(string message, Exception innerException) :
        base(message, innerException)
    {
    }
}
