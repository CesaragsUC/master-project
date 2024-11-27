using Bogus;
using Domain.Handlers.Comands;
using Domain.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
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

        [HttpPost]
        [Route("product-create-with-bogus/{total:int}")]
        [Authorize(Roles = "Create")]
        public async Task<IActionResult> AddMany(int total)
        {
            foreach (var item in await CriarListaProduto(total))
            {
                await _mediator.Send(item);
            }

            return Ok($"Foi inserido um total de {total} produtos.");
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "Update")]
        public async Task<IActionResult> Update([FromBody] UpdateProductCommand produto)
        {
            var result = await _mediator.Send(produto);

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete")]
        [Authorize(Roles = "Delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteProductCommand produto)
        {
            var result = await _mediator.Send(produto);

            return Ok(result);
        }

        private async Task<List<CreateProductCommand>> CriarListaProduto(int total)
        {
            Faker faker = new Faker("pt_BR");

            var produtos = new List<CreateProductCommand>();

            for (int i = 0; i < total; i++)
            {
                var produto = new CreateProductCommand
                {
                    Name = faker.Commerce.ProductName(),
                    Price = faker.Random.Decimal(1, 100),
                    Active = faker.Random.Bool(),
                    
                };

                produtos.Add(produto);
            }

            return produtos;
        }
    }
}
