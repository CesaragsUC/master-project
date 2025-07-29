using Bogus;
using Product.Application.Comands.Product;

namespace Product.Api.Tests;

public static class ProductFactory
{
    public static List<Domain.Models.Product> CreateProductList(int total = 10)
    {
        var products = new List<Domain.Models.Product>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            products.Add(new Domain.Models.Product
            {
                Id = Guid.NewGuid(),
                Name = faker.Commerce.ProductName(),
                Price = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool(),
                CreatAt = faker.Date.Past()
            });
        }

        return products;
    }

    public static Domain.Models.Product CreateProduct()
    {
        Faker faker = new Faker("pt_BR");

        var product = new Domain.Models.Product
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool(),
            CreatAt = faker.Date.Past()
        };


        return product;
    }

    public static List<CreateProductCommand> ProductCommands(int total = 10)
    {
        var commands = new List<CreateProductCommand>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            commands.Add(new CreateProductCommand
            {
                Name = faker.Commerce.ProductName(),
                Price = faker.Random.Decimal(1, 100),
                Active = faker.Random.Bool()
            });
        }
        return commands;
    }
}