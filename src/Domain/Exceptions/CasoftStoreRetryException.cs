using System.Diagnostics.CodeAnalysis;

namespace Product.Domain.Exceptions;


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
