using System.Diagnostics.CodeAnalysis;

namespace Billing.Domain.Execeptions;

[ExcludeFromCodeCoverage]
public class InvalidPaymentTokenException : Exception
{
    public InvalidPaymentTokenException(string message) : base(message)
    {
    }
}