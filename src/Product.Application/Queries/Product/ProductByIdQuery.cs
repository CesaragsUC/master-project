using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Queries.Product;

[ExcludeFromCodeCoverage]
public class ProductByIdQuery : IRequest<Domain.Models.Product>
{
    public Guid Id { get; set; }
}
