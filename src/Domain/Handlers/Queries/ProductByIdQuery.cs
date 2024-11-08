using MediatR;
using Domain.Models;

namespace Domain.Handlers.Queries
{
    public class ProductByIdQuery : IRequest<Product>
    {
        public Guid Id { get; set; }
    }

    public class ProductQuery : IRequest<List<Product>>
    {
    }
}
