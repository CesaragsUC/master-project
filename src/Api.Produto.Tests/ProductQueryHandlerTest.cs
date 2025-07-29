using Moq;
using Product.Application.Comands.Product;
using Product.Application.Queries.Product;

namespace Product.Api.Tests;

public class ProductQueryHandlerTest : BaseIntegrationTest
{
    public ProductQueryHandlerTest(ApiFactory factory) : base(factory)
    {
        InitializeMediatrService();
    }


    [Fact(DisplayName = "Teste 01- Rotornar lista com sucesso")]
    [Trait("ProductQueryHandler", "ProductoQueryHandlerTest")]
    public async Task Test1()
    {
        var queryCommand = new ProductQuery();

        foreach (var command in ProductFactory.ProductCommands(3))
        {
            await Sender.Send(command);
        }

        var products = await Sender.Send(queryCommand);

        // Act
        Assert.True(products.Any());

    }

    [Fact(DisplayName = "Teste 02 - Obter por Id com sucesso")]
    [Trait("ProductQueryHandler", "ProductoQueryHandlerTest")]
    public async Task Test2()
    {
        var commandCreate = new CreateProductCommand
        {
            Name = "AMD Ryzen 7 7700X",
            Price = 150.5m,
            Active = true
        };


        await Sender.Send(commandCreate);

        // Act
        var product = DbContext.Products.FirstOrDefault(p => p.Name.Equals(commandCreate.Name));

        var commandQueryById = new ProductByIdQuery { Id = product.Id };

        var result = await Sender.Send(commandQueryById);

        // Act
        Assert.NotNull(result);
    }

}
