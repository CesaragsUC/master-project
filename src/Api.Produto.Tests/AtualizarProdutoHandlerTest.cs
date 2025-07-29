using Product.Application.Comands.Product;


namespace Product.Api.Tests;


public class AtualizarProdutoHandlerTest : BaseIntegrationTest
{

    public AtualizarProdutoHandlerTest(ApiFactory factory) : base(factory)
    {
        InitializeMediatrService();
    }

    [Fact(DisplayName = "Teste 01 - Atualizar com sucesso")]
    [Trait("UpdateProductHandler", "UpdateProductHandlerTest")]
    public async Task Test1()
    {
        // Arrange

        var productCommand = new CreateProductCommand
        {
            Name = "Nitendo",
            Price = 20.00m,
            Active = false,
        };


        var createResult = await Sender.Send(productCommand);

        // Act
        var productCreated = DbContext.Products.FirstOrDefault(p => p.Name.Equals(productCommand.Name));

        var productUpdateCommand = new UpdateProductCommand
        {
            Id = productCreated.Id,
            Name = "Plastation 5",
            Price = 10.5m,
            Active = true
        };

        var updateResult = await Sender.Send(productUpdateCommand);

        var productUpdated = DbContext.Products.FirstOrDefault(p => p.Name.Equals(productUpdateCommand.Name));

        // Act
        Assert.False(productCreated.Name != productUpdated.Name);
        Assert.True(createResult.Succeeded);
        Assert.True(updateResult.Succeeded);

    }

    [Fact(DisplayName = "Teste 02 - Atualizar erro")]
    [Trait("UpdateProductHandler", "UpdateProductHandlerTest")]
    public async Task Test2()
    {
        // Arrange

        var productCommand = new CreateProductCommand
        {
            Name = "Nitendo",
            Price = 20.00m,
            Active = false,
        };


        var createResult = await Sender.Send(productCommand);

        // Act
        var productCreated = DbContext.Products.FirstOrDefault(p => p.Name.Equals(productCommand.Name));

        var productUpdateCommand = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Plastation 5",
            Price = 10.5m,
            Active = true
        };

        var updateResult = await Sender.Send(productUpdateCommand);

        var productUpdated = DbContext.Products.FirstOrDefault(p => p.Name.Equals(productUpdateCommand.Name));

        // Act
        Assert.Null(productUpdated);
        Assert.True(createResult.Succeeded);
        Assert.False(updateResult.Succeeded);
    }
}