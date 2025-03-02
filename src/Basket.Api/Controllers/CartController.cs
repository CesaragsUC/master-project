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
    [Route("save")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpsertAsync(CartDto request)
    {
        var result = await _cartService.SaveCartAsync(request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    [Route("update")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateItem(UpdateCartItemDto request)
    {
        var result = await _cartService.UpdateCartAsync(request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    [Route("remove-item/{customerId}/{productId}")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem(Guid customerId, Guid productId)
    {
        var result = await _cartService.RemoveItemAsync(customerId, productId);
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

    [HttpDelete]
    [Route("delete")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid customerId)
    {
        var discountResult = await _cartService.DeleteCart(customerId);
        return discountResult.Succeeded ? Ok(discountResult) : BadRequest(discountResult);
    }
}
