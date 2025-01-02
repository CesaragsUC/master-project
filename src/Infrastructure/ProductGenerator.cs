using Bogus;
using Product.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace Infrasctructure;

[ExcludeFromCodeCoverage]
public static class ProductGenerator
{
    public static Product.Domain.Models.Product CreateProduto()
    {
        Faker bogus = new Faker();
        return new Product.Domain.Models.Product
        {
            Id = bogus.Random.Guid(),
            Name = bogus.Commerce.ProductName(),
            Price = bogus.Random.Decimal(1, 1000),
            Active = bogus.Random.Bool(),

        };

    }

    public static Product.Domain.Models.Product CreateNullProduto()
    {
        return null;
    }

    public static List<Product.Domain.Models.Product> CreateProdutoList(int total)
    {
        var produtos = new List<Product.Domain.Models.Product>();
        for (int i = 0; i < total; i++)
        {
            produtos.Add(CreateProduto());
        }

        Faker bogus = new Faker();
        var produto = new Product.Domain.Models.Product
        {
            Id = Guid.Parse("dfa9057b-9d9e-427f-9e94-4fbd0d3d02a2"),
            Name = bogus.Commerce.ProductName(),
            Price = bogus.Random.Decimal(1, 1000),
            Active = bogus.Random.Bool(),

        };

        produtos.Add(produto);

        return produtos;

    }
}
