using Api.Catalogo.Abstractions;
using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalogo.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController(IProdutoService service) : Controller
{

    [HttpGet]
    [Route("produtos")]
    public async Task<IActionResult> Index([FromQuery] ProdutoFilter filter)
    {
        var filters = new ProdutoFilter
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
    public async Task<IActionResult> Insert(ProdutoAddDto produto)
    {
        var response = await service.Insert(produto);
        return Ok(response);
    }

    [HttpPost]
    [Route("produtos-criar-com-bogus/{total:int}")]
    public async Task<IActionResult> InsertMany(int total)
    {
        var response = await service.InsertMany(await CriarListaProduto(total));
        return Ok(response);
    }

    [HttpPut]
    [Route("produtos")]
    public async Task<IActionResult> Update(ProdutoUpdateDto produto)
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
    private ProdutoFilter filter_teste()
    {
        return new ProdutoFilter
        {
            OnlyActive = true,          // Somente produtos ativos
            MinPrice = 48.99m,              // Preço mínimo de 50
            MaxPrice = 1999.99m,             // Preço máximo de 200
            Name = "a",             // Busca produtos cujo nome contém "Camiseta"
            Page = 1,                   // Primeira página
            PageSize = 10,              // Tamanho da página (10 itens por página)
            OrderBy = "Nome",           // Ordenação pelo nome do produto
            OrderDirection = "asc",     // Direção da ordenação: ascendente
        };
    }


    [ExcludeFromCodeCoverage]
    private async Task<List<ProdutoAddDto>> CriarListaProduto(int total)
    {
        Faker faker = new Faker("pt_BR");

        var produtos = new List<ProdutoAddDto>();

        for (int i = 0; i < total; i++)
        {
            var produto = new ProdutoAddDto
            {
                Nome = faker.Commerce.ProductName(),
                Preco = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),

            };

            produtos.Add(produto);
        }

        return produtos;
    }
    #endregion
}
