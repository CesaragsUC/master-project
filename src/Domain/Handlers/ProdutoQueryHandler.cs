using Domain.Handlers.Queries;
using Domain.Interfaces;
using Product.Domain.Models;
using MediatR;

namespace Product.Domain.Handlers
{
    public class ProdutoQueryHandler : 
        IRequestHandler<ProductQuery, List<Models.Product>>,
        IRequestHandler<ProductByIdQuery, Models.Product>
    {

        private readonly IRepository<Models.Product> _repository;

        public ProdutoQueryHandler(IRepository<Models.Product> repository)
        {
            _repository = repository;
        }

        public async Task<List<Models.Product?>> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetAll().ToList()!;
        }

        public async Task<Models.Product?> Handle(ProductByIdQuery request, CancellationToken cancellationToken)
        {
            var produto = _repository.FindOne(x => x.Id == request.Id);

            if (produto == null) return null;

            return produto;
        }
    }
}
