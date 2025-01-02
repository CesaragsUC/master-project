using Bogus;

namespace Product.Api.Tests;

public static class ProductFactory
{
    public static List<Product.Domain.Models.Product> CreateProductList(int total = 10)
    {
        var products = new List<Product.Domain.Models.Product>();
        for (int i = 0; i < total; i++)
        {
            Faker faker = new Faker("pt_BR");
            products.Add(new Product.Domain.Models.Product
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

    public static Product.Domain.Models.Product CreateProduct()
    {
        Faker faker = new Faker("pt_BR");

        var product = new Product.Domain.Models.Product
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(1, 100),
            Active = faker.Random.Bool(),
            CreatAt = faker.Date.Past()
        };


        return product;
    }
}