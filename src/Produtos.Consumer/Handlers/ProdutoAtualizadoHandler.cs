using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Serilog;

namespace Produtos.Consumer.Handlers;

public class ProdutoAtualizadoHandler :
    IRequestHandler<ProdutoAtualizadoEvent, bool>
{
    private readonly IMongoRepository<ProdutoAtualizadoEvent> _repository;

    public ProdutoAtualizadoHandler(IMongoRepository<ProdutoAtualizadoEvent> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ProdutoAtualizadoEvent request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.UpdateAsync(nameof(request.ProdutoId), request, nameof(Domain.Models.Produtos));

            Log.Information("Atualizado produto nome: {Nome} - {Data}", request.Nome, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Falha ao integrar produto", ex);
            return false;
        }
    }
}
