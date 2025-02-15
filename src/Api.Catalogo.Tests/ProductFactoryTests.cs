using Bogus;
using Catalog.Application.Dtos;
using Catalog.Domain.Models;
namespace Catalogo.Api.Tests;

public static class ProductFactoryTests
{
    public static List<Products> CriarProdutoLista(int total = 10)
    {
       
        var produtos = new List<Products>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            produtos.Add(new Products
            {
                ProductId = faker.Random.Guid().ToString(),
                Name = faker.Commerce.ProductName(),
                Price = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
                CreatedAt = faker.Date.Past()
            });
        }

        return produtos;
    }

    public static Products CriarProduto()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new Products
        {
            ProductId = faker.Random.Guid().ToString(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool(),
            CreatedAt = faker.Date.Past()
        };


        return produto;
    }

    public static List<ProductDto> CriarProdutoDtoLista(int total = 10)
    {

        var produtos = new List<ProductDto>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            produtos.Add(new ProductDto
            {
                ProductId = faker.Random.Guid().ToString(),
                Name = faker.Commerce.ProductName(),
                Price = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
                CreatAt = faker.Date.Past()
            });
        }

        return produtos;
    }

    public static List<ProductCreateDto> CriarProdutoAddDtoLista(int total = 10)
    {

        var produtos = new List<ProductCreateDto>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            produtos.Add(new ProductCreateDto
            {
                Name = faker.Commerce.ProductName(),
                Price = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
            });
        }

        return produtos;
    }

 

    public static ProductDto CriarProdutoDto()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new ProductDto
        {
            ProductId = faker.Random.Guid().ToString(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool(),
            CreatAt = faker.Date.Past()
        };


        return produto;
    }

    public static ProductCreateDto CriarProdutoAddDto()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new ProductCreateDto
        {
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool()
        };


        return produto;
    }

    public static ProductCreateDto CriarProdutoAddDtoInvalido()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new ProductCreateDto
        {
            Name = string.Empty,
            Price = 0,
            Active = faker.Random.Bool()
        };


        return produto;
    }

}