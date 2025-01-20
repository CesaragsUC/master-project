using System.Net;

namespace Product.Domain.Exceptions;

public class ProductNotFoundException : BaseException
{
    public ProductNotFoundException(Guid id)
        : base($"product with id {id} not found", HttpStatusCode.NotFound)
    {
    }

    public ProductNotFoundException(string message)
    : base($"{message}", HttpStatusCode.NotFound)
    {
    }
}
