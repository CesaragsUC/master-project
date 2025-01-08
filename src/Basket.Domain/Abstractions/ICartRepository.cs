using Basket.Domain.Entities;

namespace Basket.Domain.Abstractions;

public interface ICartRepository
{
    Task UpsertAsync(Cart cart);
    Task<Cart?> GetAsync(Guid customerId);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(Guid customerId);

}
