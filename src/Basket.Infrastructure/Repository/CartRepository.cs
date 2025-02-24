using Basket.Domain.Abstractions;
using Basket.Domain.Entities;
using EasyMongoNet.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Infrastructure.Repository;

[ExcludeFromCodeCoverage]
public class CartRepository : ICartRepository
{
    private readonly IMongoRepository<Cart> _repository;

    public CartRepository(IMongoRepository<Cart> repository)
    {
        _repository = repository;
    }

    public async Task UpsertAsync(Cart cart)
    {
       await _repository.UpsertAsync(x=> x.CustomerId == cart.CustomerId, cart);
    }

    public async Task DeleteAsync(Guid customerId)
    {
        await _repository.DeleteOneAsync(x=> x.CustomerId == customerId);
    }

    public async Task<Cart?> GetAsync(Guid customerId)
    {
        return await _repository.FindOneAsync(x => x.CustomerId == customerId);
    }

    public async Task UpdateAsync(Cart cart)
    {
        await _repository.UpdateAsync(cart);
    }
}
