using Discount.Domain.Dtos;
using Discount.Domain.Entities;
using ResultNet;

namespace Discount.Domain.Abstractions;

public interface ICouponService
{
    Task<Result<IEnumerable<Coupon>>> GetAll();
    Task<DiscountResponse> ApplyDiscount(DiscountRequest discountRequest);
    Task<Result<Coupon>> GetCouponByCode(string code);
    Task<Result<bool>> CreateCoupon(CouponCreateDto coupon);
    Task<Result<bool>> UpdateCoupon(CouponUpdateDto coupon);
    Task<Result<bool>> DeleteCoupon(Guid id);
}
