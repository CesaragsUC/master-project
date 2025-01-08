using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Order.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> List()
    {
        return Ok();
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Ok();
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add()
    {
        return Ok();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> Update()
    {
        return Ok();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok();
    }
}
