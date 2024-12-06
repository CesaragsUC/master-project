using Catalog.Domain.Abstractions;
using Catalog.Domain.Models;

using MediatR;
using Messaging.Contracts.Events.Product;
using Serilog;

namespace Product.Consumer.Handlers;
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

            Log.Information("Product: {Nome} - {Data} was updated.", request.Name, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Fail to updated product", ex);
            return false;
        }
    }
}
