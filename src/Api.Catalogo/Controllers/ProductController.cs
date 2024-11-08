using Api.Catalogo.Abstractions;
using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalogo.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProductController(IProductService service) : Controller
{

    [HttpGet]
    [Route("produtos")]
    public async Task<IActionResult> Index([FromQuery] ProductFilter filter)
    {
        var filters = new ProductFilter
        {
            OnlyActive = filter.OnlyActive,
            Name = filter.Name,
            MinPrice = filter.MinPrice,
            MaxPrice = filter.MaxPrice,
            Page = filter.Page,
            PageSize = filter.PageSize,
            OrderBy = filter.OrderBy,
            OrderDirection = filter.OrderDirection
        };

        var produtos = await service.GetAll(filters);
        return Ok(produtos);
    }

    [HttpGet]
    [Route("produtos/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var produto = await service.GetById("ProdutoId",id);
        return Ok(produto);
    }

    [HttpGet]
    [Route("produtos/{nome}")]
    public async Task<IActionResult> GetByName(string nome)
    {
        var produto = await service.GetByName("Nome", nome);
        return Ok(produto);
    }

    [HttpPost]
    [Route("produtos")]
    public async Task<IActionResult> Insert(ProductCreateDto produto)
    {
        var response = await service.Insert(produto);
        return Ok(response);
    }

    [HttpPost]
    [Route("produtos-criar-com-bogus/{total:int}")]
    public async Task<IActionResult> InsertMany(int total)
    {
        var response = await service.InsertMany(await CreateListProduct(total));
        return Ok(response);
    }

    [HttpPut]
    [Route("produtos")]
    public async Task<IActionResult> Update(ProductUpdateDto produto)
    {
        var response = await service.Update(produto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("produtos/{id:guid}")]
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
