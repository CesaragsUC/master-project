using Discount.Domain.Abstractions;
using Discount.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/discount")]
public class DiscountController(ICouponService couponService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("all")]
    public async Task<IActionResult> Get()
    {
        var result = await couponService.GetAll();

        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("coupon")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await couponService.GetCouponByCode(code);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("apply")]
    public async Task<IActionResult> ApplyDiscount(string couponCode, decimal totalPrice)
    {
        var discountRequest = new DiscountRequest
        {
            Code = couponCode,
            Total = totalPrice
        };

        var result = await couponService.ApplyDiscount(discountRequest);

        return result.Succeed ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("create")]
    public async Task<IActionResult> Add(CouponCreateDto couponCreate)
    {
        var result = await couponService.CreateCoupon(couponCreate);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("update")]
    public async Task<IActionResult> Update(CouponUpdateDto couponUpdate)
    {
        var result = await couponService.UpdateCoupon(couponUpdate);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await couponService.DeleteCoupon(id);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

}
