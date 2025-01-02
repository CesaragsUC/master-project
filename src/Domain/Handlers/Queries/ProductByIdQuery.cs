using MediatR;

namespace Domain.Handlers.Queries
{
    public class ProductByIdQuery : IRequest<Product.Domain.Models.Product>
    {
        public Guid Id { get; set; }
    }

    public class ProductQuery : IRequest<List<Product.Domain.Models.Product>>
    {
    }
}
