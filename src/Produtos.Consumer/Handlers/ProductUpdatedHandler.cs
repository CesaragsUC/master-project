
using Catalog.Domain.Abstractions;
using Catalog.Domain.Models;
using MediatR;
using Messaging.Contracts.Events.Product;
using Serilog;

namespace Produtos.Consumer.Handlers;

public class ProductUpdatedHandler :
    IRequestHandler<ProductUpdatedEvent, bool>
{
    private readonly IMongoRepository<ProductUpdatedEvent> _repository;

    public ProductUpdatedHandler(IMongoRepository<ProductUpdatedEvent> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ProductUpdatedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.UpdateAsync(nameof(request.ProductId), request, nameof(Products));

            Log.Information("Atualizado produto nome: {Nome} - {Data}", request.Name, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Falha ao integrar produto", ex);
            return false;
        }
    }
}
