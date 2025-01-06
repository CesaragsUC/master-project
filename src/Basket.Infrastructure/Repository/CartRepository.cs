using Basket.Domain.Abstractions;
using Basket.Domain.Entities;
using MongoRepoNet;
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
       await _repository.UpsertAsync(nameof(cart.CustomerId),cart);
    }

    public async Task DeleteAsync(Guid customerId)
    {
        await _repository.DeleteAsync("CustomerId", customerId);
    }

    public async Task<Cart?> GetAsync(Guid customerId)
    {
       var teste = await _repository.GetByIdAsync("CustomerId", customerId);
        return teste;
    }

    public async Task UpdateAsync(Cart cart)
    {
        await _repository.UpdateAsync("CustomerId", cart);
    }
}
