using Discount.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/discount")]
public class DiscountController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("all")]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("apply")]
    public async Task<IActionResult> Add(DiscountRequest discountRequest)
    {
        return Ok();
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("create")]
    public async Task<IActionResult> Add(CouponDiscountCreateDto discountCreateDto)
    {
        return Ok();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("update")]
    public async Task<IActionResult> Update(CouponDiscountUpdateDto discountUpdateDto)
    {
        return Ok();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok();
    }
}
