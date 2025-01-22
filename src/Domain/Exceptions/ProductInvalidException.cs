using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Product.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class ProductInvalidException : BaseException
{
    public ProductInvalidException(string message)
    : base($"{message}", HttpStatusCode.InternalServerError)
    {
    }
}