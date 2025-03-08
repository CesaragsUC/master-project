using Basket.Api.Dtos;
using ResultNet;

namespace Basket.Api.Abstractions;

public interface ICartService
{
    Task<Result<CartDto?>> GetCartAsync(Guid customerId);

    Task<Result<bool>> SaveCartAsync(CartDto cartDto);

    Task<Result<bool>> UpdateCartAsync(UpdateCartItemDto cartDto);

    Task<Result<bool>> RemoveItemAsync(Guid customerId, Guid productId);
    
    Task<Result<bool>> DeleteCart(Guid customerId);

    Task<Result<bool>> CheckoutAsync(CartDto checkoutDto);

    Task<Result<DiscountResultResponse>> ApplyDiscountAsync(DiscountRequest discountRequest);

    Task<Result<bool>> UpdateTotalPriceCartAsync(Guid customerId, decimal discount);
}
