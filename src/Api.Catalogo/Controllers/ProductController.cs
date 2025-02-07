using Application.Dtos.Dtos.Produtos;
using Bogus;
using Catalog.Application.Abstractions;
using Catalog.Application.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalogo.Controllers;

[Authorize]
[Route("api/catalog")]
[ApiController]
[ExcludeFromCodeCoverage]
public class ProductController(IProductService service) : ControllerBase
{

    [HttpGet]
    [Route("all")]
    [Authorize(Roles = "Read")]
    public async Task<IActionResult> Index([FromQuery] ProductFilter filter)
    {
        var produtos = await service.GetAll(filter);
        return Ok(produtos);
    }

    [HttpGet]
    [Route("product/{id:guid}")]
    [Authorize(Roles = "Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var product = await service.GetById("ProductId", id);
        return Ok(product);
    }

    [HttpGet]
    [Route("product/{name}")]
    [Authorize(Roles = "Read")]
    public async Task<IActionResult> GetByName(string nome)
    {
        var produto = await service.GetByName(nome);
        return Ok(produto);
    }

    [HttpPost]
    [Route("product")]
    [Authorize(Roles = "Create")]
    public async Task<IActionResult> Insert(ProductCreateDto produto)
    {
        var response = await service.Insert(produto);
        return Ok(response);
    }

    [HttpPost]
    [Route("product-create-with-bogus/{total:int}")]
    [Authorize(Roles = "Create")]
    public async Task<IActionResult> InsertMany(int total)
    {
        var response = await service.InsertMany(await CreateListProduct(total));
        return Ok(response);
    }

    [HttpPut]
    [Route("product")]
    [Authorize(Roles = "Update")]
    public async Task<IActionResult> Update(ProductUpdateDto produto)
    {
        var response = await service.Update(produto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("product/{id:guid}")]
    [Authorize(Roles = "Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await service.Delete("ProdutoId",id);
        return Ok(response);
    }

    #region Testes

    [ExcludeFromCodeCoverage]
    private ProductFilter filter_teste()
    {
        return new ProductFilter
        {
            OnlyActive = true,          
            MinPrice = 48.99m,           
            MaxPrice = 1999.99m,  
            Name = "a", 
            Page = 1,  
            PageSize = 10,  
            OrderBy = "Nome",
            OrderDirection = "asc",
        };
    }


    [ExcludeFromCodeCoverage]
    private async Task<List<ProductCreateDto>> CreateListProduct(int total)
    {
        Faker faker = new Faker("pt_BR");

        var produtos = new List<ProductCreateDto>();

        for (int i = 0; i < total; i++)
        {
            var produto = new ProductCreateDto
            {
                Name = faker.Commerce.ProductName(),
                Price = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),

            };

            produtos.Add(produto);
        }

        return produtos;
    }
    #endregion
}
