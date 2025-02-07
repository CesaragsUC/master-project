using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Queries.Product;

[ExcludeFromCodeCoverage]
public class ProductQuery : IRequest<List<Domain.Models.Product>>
{
}
