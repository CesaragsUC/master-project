using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Order.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    [Authorize(Roles = "Read")]
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
}
