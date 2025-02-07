using ResultNet;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Comands.Product;

[ExcludeFromCodeCoverage]
public class DeleteProductCommand : IRequest<Result<bool>>
{
    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
}
