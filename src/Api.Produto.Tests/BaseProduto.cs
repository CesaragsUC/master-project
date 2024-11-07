using Bogus;
using Domain.Models;

namespace Tests;

public static class BaseProdutos
{
    public static List<Produtos> CriarteProdutosLista(int total = 10)
    {
        var Produtos = new List<Produtos>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            Produtos.Add(new Produtos
            {
                Id = Guid.NewGuid(),
                Nome = faker.Commerce.ProductName(),
                Preco = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
                CreatAt = faker.Date.Past()
            });
        }

        return Produtos;
    }

    public static Produtos CriarteProdutos()
    {
        Faker faker = new Faker("pt_BR");

        var Produtos = new Produtos
        {
            Id = Guid.NewGuid(),
            Nome = faker.Commerce.ProductName(),
            Preco = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool(),
            CreatAt = faker.Date.Past()
        };


        return Produtos;
    }
}