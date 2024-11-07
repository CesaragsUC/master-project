using Domain.Handlers.Queries;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Domain.Handlers
{
    public class ProdutoQueryHandler : 
        IRequestHandler<ProdutoQuery, List<Produtos>>,
        IRequestHandler<ProdutoByIdQuery,Produtos>
    {

        private readonly IRepository<Produtos> _repository;

        public ProdutoQueryHandler(IRepository<Produtos> repository)
        {
            _repository = repository;
        }

        public async Task<List<Produtos?>> Handle(ProdutoQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetAll().ToList()!;
        }

        public async Task<Produtos?> Handle(ProdutoByIdQuery request, CancellationToken cancellationToken)
        {
            var produto = _repository.FindOne(x => x.Id == request.Id);

            if (produto == null) return null;

            return produto;
        }
    }
}
