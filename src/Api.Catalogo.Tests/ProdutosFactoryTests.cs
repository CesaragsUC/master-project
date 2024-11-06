using Api.Catalogo.Dtos;
using Api.Catalogo.Models;
using Bogus;
namespace Api.Catalogo.Tests;

public static class ProdutosFactoryTests
{
    public static List<Produtos> CriarProdutoLista(int total = 10)
    {
       
        var produtos = new List<Produtos>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            produtos.Add(new Produtos
            {
                _id = faker.Random.Guid().ToString(),
                ProdutoId = faker.Random.Guid().ToString(),
                Nome = faker.Commerce.ProductName(),
                Preco = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
                CreatAt = faker.Date.Past()
            });
        }

        return produtos;
    }

    public static Produtos CriarProduto()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new Produtos
        {
            _id = faker.Random.Guid().ToString(),
            ProdutoId = faker.Random.Guid().ToString(),
            Nome = faker.Commerce.ProductName(),
            Preco = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool(),
            CreatAt = faker.Date.Past()
        };


        return produto;
    }

    public static List<ProdutoDto> CriarProdutoDtoLista(int total = 10)
    {

        var produtos = new List<ProdutoDto>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            produtos.Add(new ProdutoDto
            {
                ProdutoId = faker.Random.Guid().ToString(),
                Nome = faker.Commerce.ProductName(),
                Preco = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
                CreatAt = faker.Date.Past()
            });
        }

        return produtos;
    }

    public static List<ProdutoAddDto> CriarProdutoAddDtoLista(int total = 10)
    {

        var produtos = new List<ProdutoAddDto>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            produtos.Add(new ProdutoAddDto
            {
                Nome = faker.Commerce.ProductName(),
                Preco = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
            });
        }

        return produtos;
    }

 

    public static ProdutoDto CriarProdutoDto()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new ProdutoDto
        {
            ProdutoId = faker.Random.Guid().ToString(),
            Nome = faker.Commerce.ProductName(),
            Preco = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool(),
            CreatAt = faker.Date.Past()
        };


        return produto;
    }

    public static ProdutoAddDto CriarProdutoAddDto()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new ProdutoAddDto
        {
            Nome = faker.Commerce.ProductName(),
            Preco = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool()
        };


        return produto;
    }

    public static ProdutoAddDto CriarProdutoAddDtoInvalido()
    {
        Faker faker = new Faker("pt_BR");

        var produto = new ProdutoAddDto
        {
            Nome = string.Empty,
            Preco = 0,
            Active = faker.Random.Bool()
        };


        return produto;
    }

}