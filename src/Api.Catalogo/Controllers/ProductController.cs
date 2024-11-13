﻿using Application.Dtos.Dtos.Produtos;
using Bogus;
using Catalog.Services.Abstractions;
using Catalog.Services.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalogo.Controllers;

[Authorize]
[Route("api/catalog")]
[ApiController]
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
    public async Task<IActionResult> Get(Guid id)
    {
        var produto = await service.GetById("ProdutoId",id);
        return Ok(produto);
    }

    [HttpGet]
    [Route("product/{name}")]
    public async Task<IActionResult> GetByName(string nome)
    {
        var produto = await service.GetByName("Nome", nome);
        return Ok(produto);
    }

    [HttpPost]
    [Route("product")]
    public async Task<IActionResult> Insert(ProductCreateDto produto)
    {
        var response = await service.Insert(produto);
        return Ok(response);
    }

    [HttpPost]
    [Route("product-create-with-bogus/{total:int}")]
    public async Task<IActionResult> InsertMany(int total)
    {
        var response = await service.InsertMany(await CreateListProduct(total));
        return Ok(response);
    }

    [HttpPut]
    [Route("product")]
    public async Task<IActionResult> Update(ProductUpdateDto produto)
    {
        var response = await service.Update(produto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("product/{id:guid}")]
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
