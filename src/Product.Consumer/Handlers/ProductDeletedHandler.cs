using Catalog.Domain.Abstractions;
using Catalog.Domain.Models;

using MediatR;
using Messaging.Contracts.Events.Product;
using Serilog;

namespace Product.Consumer.Handlers;

public class ProductDeletedHandler :
    IRequestHandler<ProductDeletedEvent, bool>
{
    private readonly IMongoRepository<ProductDeletedEvent> _repository;

    public ProductDeletedHandler(IMongoRepository<ProductDeletedEvent> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ProductDeletedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Delete(nameof(request.ProductId), Guid.Parse(request.ProductId!), nameof(Products));

            Log.Information("Product {Id} was removed", request.ProductId);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex,"Failt to delete product", ex);
            return false;
        }
    }
}