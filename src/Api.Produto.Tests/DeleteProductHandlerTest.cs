using Moq;
using Product.Application.Comands.Product;


namespace Product.Api.Tests;

public class DeleteProductHandlerTest : BaseIntegrationTest
{
    public DeleteProductHandlerTest(ApiFactory factory) : base(factory)
    {
        InitializeMediatrService();
    }

    [Fact(DisplayName = "Teste 01 - Deletar com sucesso")]
    [Trait("ProductDeleteHandler", "DeleteProductHandlerTest")]
    public async Task Test1()
    {

        var command = new CreateProductCommand
        {
            Name = "Nissan Sentra",
            Price = 150.5m,
            Active = true
        };


        var result = await Sender.Send(command);

        var productCreated = DbContext.Products.FirstOrDefault(p => p.Name.Equals(command.Name));

        var deleteCommand = new DeleteProductCommand(productCreated.Id);

        var deleteResult = await Sender.Send(deleteCommand);


        var productDeleted = DbContext.Products.FirstOrDefault(p => p.Id == productCreated.Id);

        // Act
        Assert.Null(productDeleted);

    }

    [Fact(DisplayName = "Teste 02 - Deletar com erro")]
    [Trait("ProductDeleteHandler", " DeleteProductHandlerTest")]
    public async Task Test2()
    {
        var command = new CreateProductCommand
        {
            Name = "Nissan Sentra",
            Price = 150.5m,
            Active = true
        };


        var result = await Sender.Send(command);

        var productCreated = DbContext.Products.FirstOrDefault(p => p.Name.Equals(command.Name));

        var deleteCommand = new DeleteProductCommand(Guid.NewGuid());

        var deleteResult = await Sender.Send(deleteCommand);

        // Act
        Assert.False(deleteResult.Succeeded);
    }
}