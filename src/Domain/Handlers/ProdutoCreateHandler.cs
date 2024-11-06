using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using MassTransit;
using MediatR;
using Serilog;

namespace Domain.Handlers
{
    public class ProdutoCreateHandler : IRequestHandler<CreateProdutoCommand, bool>
    {
        private readonly IRepository<Produto> _repository;
        private readonly IPublishEndpoint _publish;

        public ProdutoCreateHandler(IRepository<Produto> repository,
            IPublishEndpoint publish)
        {
            _repository = repository;
            _publish = publish;
        }

        public async Task<bool> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null || (request.Preco <= 0 || string.IsNullOrEmpty(request.Nome)))
                    return false;

                var produto = new Produto
                {
                    Nome = request.Nome,
                    Preco = request.Preco,
                    Active = request.Active,
                    CreatAt = DateTime.Now
                };

                await _repository.Add(produto);

                await _publish.Publish<Produto>(new Produto
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    Active = produto.Active,
                    CreatAt = produto.CreatAt
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao adicionar produto");
                return false;
            }


            return true;
        }

    }
}
