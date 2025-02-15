using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Abstractions;
using Order.Application.Dto;
using System.Diagnostics.CodeAnalysis;

namespace Order.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> List()
    { 
        var result = await orderService.List();
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await orderService.Get(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add(CreateOrderDto model)
    {
        var result = await orderService.Add(model);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> Update(UpdateOrderDto model)
    {
        var result = await orderService.Update(model);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await orderService.Get(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
