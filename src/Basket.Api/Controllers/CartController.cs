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
    public async Task<IActionResult> Checkout(CartDto request)
    {
        //TODO: enviar dados do carrinho para a fila de pedidos
        return Ok();
    }

    [HttpPost]
    [Route("discount")]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Cart?>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Discount(CartDto request)
    {
        //TODO: enviar dados  do cupom para a API de descontos e retornar o carrinho com o desconto
        // testar com Refit
        return Ok();
    }
}
