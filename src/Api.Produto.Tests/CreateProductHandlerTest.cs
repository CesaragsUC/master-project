using Product.Application.Comands.Product;

//https://goatreview-com.cdn.ampproject.org/c/s/goatreview.com/mediatr-quickly-test-handlers-with-unit-tests/amp/

namespace Product.Api.Tests;


public class CreateProductHandlerTest : BaseIntegrationTest
{

    public CreateProductHandlerTest(ApiFactory factory) : base(factory)
    {
        InitializeMediatrService();
    }

    [Fact(DisplayName = "Teste 01 - Com sucesso")]
    [Trait("ProductoCreateHandler", "ProductoCreateHandlerTest")]
    public async Task Test1()
    {
        // Arrange

        var command = new CreateProductCommand
        {
            Name = "AMD Ryzen 7 7700X",
            Price = 150.5m,
            Active = true
        };


        var result = await Sender.Send(command);

        // Act
        var product = DbContext.Products.FirstOrDefault(p => p.Name.Equals(command.Name));


        //Assert

        Assert.NotNull(product);
    }


    [Fact(DisplayName = "Teste 02 - Com Falha")]
    [Trait("ProductoCreateHandler", "ProductoCreateHandlerTest")]
    public async Task Test2()
    {
        // Arrange

        var command = new CreateProductCommand();

        var result = await Sender.Send(command);

        // Act
        Assert.False(result.Succeeded);

    }

}