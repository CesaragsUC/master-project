using MediatR;

namespace Product.Application.Queries.Product;

public class ProductByIdQuery : IRequest<Domain.Models.Product>
{
    public Guid Id { get; set; }
}

public class ProductQuery : IRequest<List<Domain.Models.Product>>
{
}
