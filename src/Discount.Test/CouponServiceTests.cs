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

    [Fact(DisplayName = "Test 8 - try apply discount with a expired coupon should failure")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task TryApplyDiscount_InvalidCouponCode_ShouldFailure()
    {
        var discountRequest = new DiscountRequest { Code = "CODE001", Total = 100 };
        var coupon = new Coupon { Code = "CODE001", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, EndDate = DateTime.UtcNow.AddDays(1), TotalUse = 10, MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);

        var result = await _couponService.ApplyDiscount(discountRequest);

        Assert.False(result.Succeeded);
        Assert.Equal("Coupon Expired or is not valid", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 9 - Apply discount with a coupon type value should return success")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task ApplyDiscount_WithTypeValue_ShouldReturnSuccess()
    {
        var discountRequest = new DiscountRequest { Code = "CODE001", Total = 100 };
        var coupon = new Coupon { Code = "CODE001", MinValue = 50, Type = (int)DiscountType.Value, Value = 10, Active = true, EndDate = DateTime.UtcNow.AddDays(1), TotalUse = 1, MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);

        var result = await _couponService.ApplyDiscount(discountRequest);

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "Test 10 - Try apply discount but not purchase value is less than coupon min value should failure")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task ApplyDiscount_WithLessValueCoupon_ShouldFailue()
    {
        var discountRequest = new DiscountRequest { Code = "CODE001", Total = 100 };
        var coupon = new Coupon { Code = "CODE001", MinValue = 500, Type = (int)DiscountType.Value, Value = 10, Active = true, EndDate = DateTime.UtcNow.AddDays(1), TotalUse = 1, MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);

        var result = await _couponService.ApplyDiscount(discountRequest);

        Assert.False(result.Succeeded);
    }

    [Fact(DisplayName = "Test 11 - Invalid coupon code should failure")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task InvalidCopunCode_ShouldFailure()
    {
        var discountRequest = new DiscountRequest { Code = "CODE001", Total = 100 };

        _repositoryMock.Setup(x => x.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns((Coupon)null);
        var result = await _couponService.ApplyDiscount(discountRequest);

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid Coupon", result.Messages.FirstOrDefault());
    }


    [Fact(DisplayName = "Test 12 - Create a coupon Should throw a execption")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task CreateCoupon_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var couponCreateDto = new CouponCreateDto
        {
            Code = "TESTCODE",
            Value = 10,
            MinValue = 50,
            Type = DiscountType.Value,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            MaxUse = 100,
            Active = true
        };

        _mapperMock.Setup(m => m.Map<Coupon>(It.IsAny<CouponCreateDto>()))
                   .Throws(new Exception("Test exception"));

        var result = await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _couponService.CreateCoupon(couponCreateDto);
        });


        // Assert
        Assert.Equal("Test exception", result.Message);
    }

    [Fact(DisplayName = "Test 12 - Create a invalid coupon Should fail")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task CreateCoupon_ShouldFail()
    {
        // Arrange
        var couponCreateDto = new CouponCreateDto
        {
            Code = "TESTCODE",
            Value = 10,
            MinValue = 50,
            EndDate = DateTime.UtcNow.AddDays(10),
            MaxUse = 100,
            Active = true
        };

        _mapperMock.Setup(m => m.Map<Coupon>(It.IsAny<CouponCreateDto>()))
                   .Throws(new Exception("Test exception"));

        var result = await _couponService.CreateCoupon(couponCreateDto);
        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Messages.Count > 0);
    }

    [Fact(DisplayName = "Test 13 - update a invalid coupon Should fail")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task UpdateInvalidCoupon_ShouldFail()
    {
        var couponUpdateDto = new CouponUpdateDto { Id = Guid.NewGuid(), Code = "CASOFT", MinValue = 20, Type = DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };
        var coupon = new Coupon { Id = couponUpdateDto.Id, Code = "CASOFT30", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns(coupon);
        var result = await _couponService.UpdateCoupon(couponUpdateDto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Messages.Count > 0);
    }

    [Fact(DisplayName = "Test 14 - update a invalid coupon Should throw exception")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task UpdateInvalidCoupon_ShouldException()
    {

        var couponUpdateDto = new CouponUpdateDto { Id = Guid.NewGuid(), Code = "CASOFT30", MinValue = 20, Type = DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };
        var coupon = new Coupon { Id = couponUpdateDto.Id, Code = "CASOFT30", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Throws(new Exception("Error to updated coupon"));

        var result = await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _couponService.UpdateCoupon(couponUpdateDto);
        });

        // Assert
        Assert.Equal("Error to updated coupon", result.Message);
    }

    [Fact(DisplayName = "Test 15 - Update a coupon not founded")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task UpdateNotFoundedCoupon_ShouldFail()
    {
        var couponUpdateDto = new CouponUpdateDto { Id = Guid.NewGuid(), Code = "CASOFT30", MinValue = 20, Type = DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };
        var coupon = new Coupon { Id = couponUpdateDto.Id, Code = "CASOFT30", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns((Coupon)null);

        var result = await _couponService.UpdateCoupon(couponUpdateDto);


        // Assert
        Assert.False(result.Succeeded); 
        Assert.Equal("Coupon not found", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 16 - Delete a coupon not founded")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task DeleteNotFoundedCoupon_ShouldFail()
    {
        var coupon = new Coupon { Id = Guid.NewGuid(), Code = "CASOFT30", MinValue = 50, Type = (int)DiscountType.Percentage, Value = 10, Active = true, StartDate = DateTime.Now, EndDate = DateTime.UtcNow.AddDays(1), MaxUse = 5 };

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns((Coupon)null);

        var result = await _couponService.DeleteCoupon(Guid.NewGuid());


        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Coupon not found", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 17 - Delete a coupon with exception")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task DeleteNotCoupon_ShouldException()
    {
        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>()))
            .Throws(new Exception("Error to delete coupon"));

        // Assert
        var result = await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _couponService.DeleteCoupon(Guid.NewGuid());
        });

        Assert.Equal("Error to delete coupon", result.Message);

    }


    [Fact(DisplayName = "Test 18 - Delete a coupon empty Id")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task DeleteCouponEmpetyId_ShouldFail()
    {
        var result = await _couponService.DeleteCoupon(Guid.Empty);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Invalid id", result.Messages.FirstOrDefault());
    }


    [Fact(DisplayName = "Test 20 - Return a null list copuns")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task GetAllException_NullList()
    {
        var coupons = new List<Coupon>
        {}.AsQueryable();

        _repositoryMock.Setup(r => r.GetAllEntities(It.IsAny<FindOptions?>()))
            .Returns(coupons);

        var result = await _couponService.GetAll();

        Assert.False(result.Succeeded);
    }

    [Fact(DisplayName = "Test 19 - Get all copun exception")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task GetAllException()
    {
        
        _repositoryMock.Setup(r => r.GetAllEntities(It.IsAny<FindOptions?>())).Throws(new Exception("Error to retrive coupons"));

        // Assert
        var result = await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _couponService.GetAll();
        });

        Assert.Equal("Error to retrive coupons", result.Message);
    }

    [Fact(DisplayName = "Test 21 - Get coupon by invalid code")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task GetCouponBy_InvalidCode()
    {

         var result =  await _couponService.GetCouponByCode("");

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid coupon code", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 21 - Coupon not founded")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task GetCouponByCode_NotFounded()
    {

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Returns((Coupon)null);

        // Assert

        var result = await _couponService.GetCouponByCode("CODE001");

        Assert.False(result.Succeeded);
        Assert.Equal("Coupon code invalid or not found", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 22 - Get coupon by code with exception")]
    [Trait("Discount.Service", "Apply Discount")]
    public async Task GetCouponByCode_Exception()
    {

        _repositoryMock.Setup(r => r.FindOne(It.IsAny<Expression<Func<Coupon, bool>>>(), It.IsAny<FindOptions?>())).Throws(new Exception("Error to retrive coupon data"));
        // Assert
        var result = await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _couponService.GetCouponByCode("CODE001");
        });

        Assert.Equal("Error to retrive coupon data", result.Message);
    }
}
