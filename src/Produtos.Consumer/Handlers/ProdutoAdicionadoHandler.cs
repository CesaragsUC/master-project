using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Serilog;

namespace Produtos.Consumer.Handlers;

public class ProdutoAdicionadoHandler :
    IRequestHandler<ProdutoAdicionadoEvent, bool>
{
    private readonly IMongoRepository<ProdutoAdicionadoEvent> _repository;

    public ProdutoAdicionadoHandler(IMongoRepository<ProdutoAdicionadoEvent> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ProdutoAdicionadoEvent request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.InsertAsync(request,nameof(Domain.Models.Produtos));

            Log.Information("Adicionado produto nome: {Nome} - {Data}", request.Nome, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Falha ao integrar produto", ex);
            return false;
        }

    }
}
