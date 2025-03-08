using EasyMongoNet.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Product;
using Product.Application.MongoEntities;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class ProductDeletedEventHandler :
    IRequestHandler<ProductDeletedEvent, bool>
{
    private readonly IMongoRepository<Products> _repository;

    public ProductDeletedEventHandler(IMongoRepository<Products> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ProductDeletedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ProductId == null)
            {
                Log.Error("Product Id is null");
                return false;
            }

            await _repository.DeleteOneAsync(x => x.ProductId!.Equals(request.ProductId));

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to delete product", ex);
            return false;
        }
    }
}