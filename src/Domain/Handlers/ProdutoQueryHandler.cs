using Domain.Handlers.Queries;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Domain.Handlers
{
    public class ProdutoQueryHandler : 
        IRequestHandler<ProductQuery, List<Product>>,
        IRequestHandler<ProductByIdQuery, Product>
    {

        private readonly IRepository<Product> _repository;

        public ProdutoQueryHandler(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<List<Product?>> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetAll().ToList()!;
        }

        public async Task<Product?> Handle(ProductByIdQuery request, CancellationToken cancellationToken)
        {
            var produto = _repository.FindOne(x => x.Id == request.Id);

            if (produto == null) return null;

            return produto;
        }
    }
}
