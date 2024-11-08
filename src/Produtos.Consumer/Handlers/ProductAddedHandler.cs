using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Serilog;

namespace Produtos.Consumer.Handlers;

public class ProductAddedHandler :
    IRequestHandler<ProductAddedEvent, bool>
{
    private readonly IMongoRepository<ProductAddedEvent> _repository;

    public ProductAddedHandler(IMongoRepository<ProductAddedEvent> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ProductAddedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.InsertAsync(request,nameof(Domain.Models.Product));

            Log.Information("Adicionado produto nome: {Nome} - {Data}", request.Name, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Falha ao integrar produto", ex);
            return false;
        }

    }
}
