using ResultNet;
using MediatR;

namespace Product.Application.Comands.Product;

public class DeleteProductCommand : IRequest<Result<bool>>
{
    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
}
