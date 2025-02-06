using Basket.Api.Abstractions;
using Basket.Api.Dtos;
using Basket.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using ResultNet;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartRepository)
    {
        _cartService = cartRepository;
    }

    [HttpGet]
    [Route("{customerId:guid}")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(Guid customerId)
    {
        var result = await _cartService.GetCartAsync(customerId);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    [Route("upsert")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpsertAsync(CartDto request)
    {
        var result = await _cartService.SaveOrUpdateCartAsync(request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }


    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout(CartDto cartDto)
    {
        var response = await _cartService.CheckoutAsync(cartDto);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    [Route("discount")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Discount(CartDto cart)
    {
        var discountResult = await _cartService.ApplyDiscountAsync(cart);
        return discountResult.Succeeded ? Ok(discountResult) : BadRequest(discountResult);
    }
}
