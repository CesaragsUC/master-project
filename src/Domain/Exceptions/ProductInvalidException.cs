using System.Net;

namespace Product.Domain.Exceptions;

public class ProductInvalidException : BaseException
{
    public ProductInvalidException(string message)
    : base($"{message}", HttpStatusCode.InternalServerError)
    {
    }
}