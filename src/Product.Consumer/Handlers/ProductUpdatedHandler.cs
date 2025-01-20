using AutoMapper;
using EasyMongoNet.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Product;
using Product.Consumer.Models;
using Serilog;

namespace Product.Consumer.Handlers;
public class ProductUpdatedHandler :
    IRequestHandler<ProductUpdatedEvent, bool>
{
    private readonly IMongoRepository<Products> _repository;
    private readonly IMapper _mapper;

    public ProductUpdatedHandler(IMongoRepository<Products> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(ProductUpdatedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var product = _mapper.Map<Products>(request);

            var productDb = await _repository.FindOneAsync(x => x.ProductId == product.ProductId);


            if (productDb == null)
            {
                Log.Error("Product not found: {ProductId}", product.ProductId);
                return false;
            }

            productDb.ProductId = product.ProductId;
            productDb.Name = product.Name;
            productDb.Price = product.Price;
            productDb.Active = product.Active;
            productDb.ImageUri = product.ImageUri;
            productDb.ModifiedAt = DateTime.Now;
            await _repository.UpdateAsync(productDb);

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
