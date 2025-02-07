using System.Diagnostics.CodeAnalysis;

namespace Order.Domain.Execeptions;

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
