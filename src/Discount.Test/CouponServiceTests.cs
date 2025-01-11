namespace Discount.Test;

using AutoMapper;
using Discount.Api.Services;
using Discount.Domain.Dtos;
using Discount.Domain.Entities;
using Discount.Domain.ValueObjects;
using Moq;
using RepoPgNet;
using System.Linq.Expressions;
using Xunit;

public class CouponServiceTests
{
    private readonly Mock<IPgRepository<Coupon>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CouponService _couponService;

    public CouponServiceTests()
    {
        _repositoryMock = new Mock<IPgRepository<Coupon>>();
        _mapperMock = new Mock<IMapper>();
        _couponService = new CouponService(_repositoryMock.Object, _mapperMock.Object);
    }


    [Fact(DisplayName = "Test 1  apply discount invalid request returns failure")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task ApplyDiscount_InvalidRequest_ReturnsFailure()
    {
        var discountRequest = new DiscountRequest { Code = null, Total = 0 };
        var result = await _couponService.ApplyDiscount(discountRequest);
        Assert.False(result.Succeeded);
        Assert.Equal("Invalid request", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 2  apply discount valid request returns success")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task ApplyDiscount_ValidRequest_ReturnsSuccess()
    {
        var discountRequest = new DiscountRequest { Code = "TEST", Total = 100 };
        var coupon = new Coupon { Code = "TEST", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, EndDate = DateTime.UtcNow.AddDays(1), TotalUse = 0, MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);
        var result = await _couponService.ApplyDiscount(discountRequest);

        Assert.True(result.Succeeded);
        Assert.Equal(10, result.Data.TotalDiscount);
    }

    [Fact(DisplayName = "Test 3 create coupon valid coupon returns success")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task CreateCoupon_ValidCoupon_ReturnsSuccess()
    {
        var couponCreateDto = new CouponCreateDto { Code = "CASOFT20", MinValue = 50, Type = DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };
        var coupon = new Coupon { Code = "CASOFT20", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };

        _mapperMock.Setup(m => m.Map<Coupon>(couponCreateDto)).Returns(coupon);
        var result = await _couponService.CreateCoupon(couponCreateDto);

        Assert.True(result.Succeeded);
        Assert.Equal("Coupon created successfully", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 4 update coupon valid coupon returns success")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task UpdateCoupon_ValidCoupon_ReturnsSuccess()
    {
        var couponUpdateDto = new CouponUpdateDto { Id = Guid.NewGuid(), Code = "CASOFT20", MinValue = 20, Type = DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };
        var coupon = new Coupon { Id = couponUpdateDto.Id, Code = "CASOFT30", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);
        var result = await _couponService.UpdateCoupon(couponUpdateDto);

        Assert.True(result.Succeeded);
        Assert.Equal("Coupon updated successfully", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 5 delete coupon valid id returns success")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task DeleteCoupon_ValidId_ReturnsSuccess()
    {
        var coupon = new Coupon { Id = Guid.NewGuid(), Code = "TEST" };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);

        var result = await _couponService.DeleteCoupon(coupon.Id);

        Assert.True(result.Succeeded);
        Assert.Equal("Coupon deleted successfully", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 6 get all coupons returns coupons")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task GetAll_ReturnsCoupons()
    {
        var coupons = new List<Coupon> { new Coupon { Code = "TEST" } };

        _repositoryMock.Setup(r => r.GetAllEntities(It.IsAny<FindOptions?>())).Returns(coupons.AsQueryable());

        var result = await _couponService.GetAll();

        Assert.True(result.Succeeded);
        Assert.NotEmpty(result.Data);
    }


    [Fact(DisplayName = "Test 7 get coupon by code valid code returns coupon")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task GetCouponByCode_ValidCode_ReturnsCoupon()
    {
        var coupon = new Coupon { Code = "TEST" };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);
        var result = await _couponService.GetCouponByCode("TEST");

        Assert.True(result.Succeeded);
        Assert.Equal(coupon, result.Data);
    }
}
