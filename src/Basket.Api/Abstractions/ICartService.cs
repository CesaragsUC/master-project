using Basket.Api.Dtos;
using Basket.Domain.Entities;
using ResultNet;

namespace Basket.Api.Abstractions;

public interface ICartService
{
    Task<Result<Cart?>> GetCartAsync(Guid customerId);
    Task<Result<bool>> SaveOrUpdateCartAsync(CartDto cartDto);
}
