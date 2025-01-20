using Domain.Handlers.Comands;
using Domain.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/product")]
public class ProductController(IMediator _mediator) : ControllerBase
{

    [Route("all")]
    [HttpGet]
    [Authorize(Roles = "Read")]
    public async Task<IActionResult> Get([FromQuery] ProductQuery produto)
    {
        var result = await _mediator.Send(produto);

        return Ok(result);
    }

    [Route("get")]
    [HttpGet]
    [Authorize(Roles = "Read")]
    public async Task<IActionResult> Get([FromQuery] ProductByIdQuery produto)
    {
        var result = await _mediator.Send(produto);

        return Ok(result);
    }

    [HttpPost]
    [Route("add")]
    [Authorize(Roles = "Create")]
    public async Task<IActionResult> Add([FromBody] CreateProductCommand produto)
    {
        var result = await _mediator.Send(produto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }


    [HttpPut]
    [Route("update")]
    [Authorize(Roles = "Update")]
    public async Task<IActionResult> Update([FromBody] UpdateProductCommand produto)
    {
        var result = await _mediator.Send(produto);

        return result.Succeeded ?  Ok(result) : BadRequest(result);
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize(Roles = "Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));

        return result.Succeeded ?  Ok(result) : BadRequest(result);
    }

}
