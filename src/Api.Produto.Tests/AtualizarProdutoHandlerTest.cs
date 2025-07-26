using Domain.Interfaces;
using Moq;
using Product.Application.Comands.Product;
using Product.Application.Handlers.Product;
using Product.Domain.Abstractions;


namespace Product.Api.Tests;

public class AtualizarProdutoHandlerTest : BaseIntegrationTest
{

    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<IBobStorageService> _bobStorageService;
    private readonly Mock<IProductService> _productService;
    private readonly UpdateProductHandler _handler;
    public AtualizarProdutoHandlerTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        InitializeMediatrService();

        _productService = new Mock<IProductService>();
        _bobStorageService = new Mock<IBobStorageService>();
        _productRepository = new Mock<IProductRepository>();

        _handler = new UpdateProductHandler(_bobStorageService.Object, _productService.Object, _productRepository.Object);
    }

    [Fact(DisplayName = "Teste 01 - Atualizar com sucesso")]
    [Trait("Produtoservice", " ProductUpdateHandler")]
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
    [Trait("Produtoservice", " ProductUpdateHandler")]
    public async Task Test2()
    {
        // Arrange

        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Produtos 01",
            Price = 10.5m,
            Active = true
        };

        var product = new Domain.Models.Product
        {
            Id = command.Id,
            Name = "Produtos Teste",
            Price = 20.00m,
            Active = true,
            CreatAt = DateTime.Now
        };

        //return null
        _productRepository.Setup(r => r.FindOne(It.IsAny<Guid>())).Returns((Domain.Models.Product)null);


        _productRepository.Setup(r => r.Update(It.IsAny<Domain.Models.Product>()))
        .Callback<Domain.Models.Product>((product) =>
        {
            // Se o produto for válido, retorne o produto
            if (product != null)
            {
                return;
            }
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.False(result.Succeeded);
        _productRepository.Verify(r => r.Update(It.IsAny<Domain.Models.Product>()), Times.Never);
    }
}