using Basket.Api.Dtos;
using Refit;

namespace Basket.Api.Abstractions;

public interface IDiscountApi
{
    [Post("/apply")]
    Task<ApiResponse<DiscountResponse>> ApplyDiscountAsync(string couponCode, decimal totalPrice);
}