using AutoMapper;
using Discount.Api.Validator;
using Discount.Domain.Abstractions;
using Discount.Domain.Dtos;
using Discount.Domain.Entities;
using Discount.Domain.ValueObjects;
using Discount.Infrastructure;
using HybridRepoNet.Abstractions;
using ResultNet;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Api.Services;

[ExcludeFromCodeCoverage]
public class CouponService : ICouponService
{
    private readonly IUnitOfWork<CouponsDbContext> _unitOfWork;

    private readonly IMapper _mapper;

    public CouponService(IUnitOfWork<CouponsDbContext> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DiscountResponse> ApplyDiscount(DiscountRequest discountRequest)
    {
        if(discountRequest.Code is null || discountRequest.Total <= 0)
        {
            return new DiscountResponse 
            { 
                Message = "Code or purchase value can't be null",
                Succeed = false
            };
        }

        var coupon = _unitOfWork.Repository<Coupon>().FindOne(x => x.Code == discountRequest.Code);

        var orderDiscount = new DiscountResponse();

        if (coupon is not null)
        {
            if (!IsValidCoupon(coupon))
            {
                return new DiscountResponse
                {
                    Message = "Coupon Expired or is not valid",
                    Succeed = false
                };
            }

            if (discountRequest.Total >= coupon.MinValue)
            {
                switch (coupon.Type)
                {
                    case (int)DiscountType.Percentage:
                        orderDiscount.TotalDiscount = Math.Round((discountRequest.Total * coupon.Value) / 100, 2);
                        break;

                    case (int)DiscountType.Value:
                        orderDiscount.TotalDiscount = coupon.Value;
                        break;
                }

                await IncreaseCouponUse(coupon);

                orderDiscount.Succeed = true;
                return orderDiscount;
            }
            else
            {
                return new DiscountResponse
                {
                    Message = "$The purchase value must be at least {coupon.MinValue:C} to use this coupon",
                    Succeed = false
                };
            }
        }
        else
        {
            return new DiscountResponse
            {
                Message = "Invalid Coupon",
                Succeed = false
            };
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

            await _unitOfWork.Repository<Coupon>().AddAsync(couponEntity);
            await _unitOfWork.Commit();

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

            var couponEntity = _unitOfWork.Repository<Coupon>().FindOne(x => x.Id == coupon.Id);

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

                _unitOfWork.Repository<Coupon>().UpdateAsync(couponEntity);
                await _unitOfWork.Commit();

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

            var couponEntity = _unitOfWork.Repository<Coupon>().FindOne(x => x.Id == id);

            if (couponEntity is not null)
            {
                _unitOfWork.Repository<Coupon>().DeleteAsync(couponEntity);
                await _unitOfWork.Commit();

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
            var couponList = await  _unitOfWork.Repository<Coupon>().GetAllAsync();

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
                return await Result<Coupon>.FailureAsync("Invalid coupon code");
            }

            var couponEntity = _unitOfWork.Repository<Coupon>().FindOne(x => x.Code.Equals(code));

            if (couponEntity is not null)
            {
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
        _unitOfWork.Repository<Coupon>().UpdateAsync(coupon);
        await _unitOfWork.Commit();
    }

    private static bool IsValidCoupon(Coupon coupon)
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
