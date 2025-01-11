using AutoMapper;
using Discount.Api.Validator;
using Discount.Domain.Abstractions;
using Discount.Domain.Dtos;
using Discount.Domain.Entities;
using Discount.Domain.ValueObjects;
using RepoPgNet;
using ResultNet;
using Serilog;
using static Azure.Core.HttpHeader;

namespace Discount.Api.Services;

public class CouponService : ICouponService
{
    private readonly IPgRepository<Coupon> _repository;

    private readonly IMapper _mapper;

    public CouponService(IPgRepository<Coupon> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<DiscountResponse>> ApplyDiscount(DiscountRequest discountRequest)
    {
        if(discountRequest.Code is null || discountRequest.Total <= 0)
        {
            return await Result<DiscountResponse>.FailureAsync("Invalid request");
        }

        var coupon = _repository.FindOne(x => x.Code == discountRequest.Code);

        var orderDiscount = new DiscountResponse();

        if (coupon is not null)
        {
            if (! await IsValidCoupon(coupon))
            {
                return await Result<DiscountResponse>.FailureAsync("Coupon Expired or is not valid");
            }

            if (discountRequest.Total >= coupon.MinValue)
            {
                switch (coupon.Type)
                {
                    case (int)DiscountType.Percentage:
                        orderDiscount.TotalDiscount = (discountRequest.Total * coupon.Value) / 100;
                        break;

                    case (int)DiscountType.Value:
                        orderDiscount.TotalDiscount = coupon.Value;
                        break;
                }

                await IncreaseCouponUse(coupon);

                return await Result<DiscountResponse>.SuccessAsync(orderDiscount);
            }
            else
            {
                return await Result<DiscountResponse>.FailureAsync($"The purchase value must be at least {coupon.MinValue:C} to use this coupon");
            }
        }
        else
        {
            return await Result<DiscountResponse>.FailureAsync($"Invalid Coupon");
        }

    }

    public async Task<Result<bool>> CreateCoupon(CouponCreateDto coupon)
    {
        try
        {
            var validator = new CouponCreateDtoValidator();

            var validationResult = await validator.ValidateAsync(coupon);

            if (!validationResult.IsValid)
            {
                return await Result<bool>.FailureAsync(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            }

            var couponEntity = _mapper.Map<Coupon>(coupon);

            await _repository.AddAsync(couponEntity);

            return await Result<bool>.SuccessAsync("Coupon created successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating coupon");
            throw;
        }

    }

    public async Task<Result<bool>> UpdateCoupon(CouponUpdateDto coupon)
    {
        try
        {
            var validator = new CouponUpdateDtoValidator();

            var validationResult = await validator.ValidateAsync(coupon);

            if (!validationResult.IsValid)
            {
                return await Result<bool>.FailureAsync(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            }

            var couponEntity = _repository.FindOne(x => x.Id == coupon.Id);

            if (couponEntity is not null)
            {
                couponEntity.Active = coupon.Active;
                couponEntity.Code = coupon.Code;
                couponEntity.EndDate = coupon.EndDate;
                couponEntity.MinValue = coupon.MinValue;
                couponEntity.Type = (int)coupon.Type;
                couponEntity.UpdatedAt = DateTime.UtcNow;
                couponEntity.Value = coupon.Value;
                couponEntity.MaxUse = coupon.MaxUse;

                await _repository.UpdateAsync(couponEntity);

                return await Result<bool>.SuccessAsync("Coupon updated successfully");
            }
            else
            {
                return await Result<bool>.FailureAsync("Coupon not found");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to updated coupon");
            throw;
        }
    }

    public async Task<Result<bool>> DeleteCoupon(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return await Result<bool>.FailureAsync("Invalid id");
            }

            var couponEntity = _repository.FindOne(x => x.Id == id);

            if (couponEntity is not null)
            {
                await _repository.DeleteAsync(couponEntity);
                return await Result<bool>.SuccessAsync("Coupon deleted successfully");
            }
            else
            { 
                return await Result<bool>.FailureAsync("Coupon not found");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to delete coupon");
            throw;
        }
    }

    public async Task<Result<IEnumerable<Coupon>>> GetAll()
    {
        try
        {
            var couponList = _repository.GetAllEntities();

            if (couponList.Any())
            {
                return await Result<IEnumerable<Coupon>>.SuccessAsync(couponList);
            }
            else
            {
                return await Result<IEnumerable<Coupon>>.FailureAsync("No coupons found.");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to retrive coupons");
            throw;
        }
    }

    public async Task<Result<Coupon>> GetCouponByCode(string code)
    {
        try
        {
            if (string.IsNullOrEmpty(code))
            {
                return await Result<Coupon>.FailureAsync("Invalid code");
            }

            var couponEntity = _repository.FindOne(x => x.Code.Equals(code));

            if (couponEntity is not null)
            {
                await _repository.DeleteAsync(couponEntity);
                return await Result<Coupon>.SuccessAsync(couponEntity);
            }
            else
            {
                return await Result<Coupon>.FailureAsync("Coupon code invalid or not found");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to retrive coupon data");
            throw;
        }
    }

    private async Task IncreaseCouponUse(Coupon coupon)
    {
        coupon.TotalUse++;
        await _repository.UpdateAsync(coupon);
    }

    private async Task<bool> IsValidCoupon(Coupon coupon)
    {
        if (coupon.EndDate < DateTime.UtcNow 
            || !coupon.Active 
            || coupon.TotalUse > coupon.MaxUse)
        {
            return false;
        }

        return true;
    }

}
