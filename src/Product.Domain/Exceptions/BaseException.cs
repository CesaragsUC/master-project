using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Product.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public BaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
    }
}