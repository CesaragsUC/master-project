using Domain.Interfaces;
using Moq;
using Product.Application.Comands.Product;
using Product.Application.Handlers.Product;
using Product.Domain.Abstractions;

//https://goatreview-com.cdn.ampproject.org/c/s/goatreview.com/mediatr-quickly-test-handlers-with-unit-tests/amp/

namespace Product.Api.Tests;

public class CreateProductHandlerTest : BaseIntegrationTest
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<IProductService> _productService;
    private readonly Mock<IBobStorageService> _bobStorageService;
    private readonly CreateProductHandler _handler;
    public CreateProductHandlerTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        InitializeMediatrService();

        _productRepository = new Mock<IProductRepository>();
        _productService = new Mock<IProductService>();
        _bobStorageService = new Mock<IBobStorageService>();
        _handler = new CreateProductHandler(_bobStorageService.Object, _productService.Object, _productRepository.Object);
    }

    [Fact(DisplayName = "Teste 01 - Com sucesso")]
    [Trait("Produtoservice", "ProductoCreateHandler")]
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
    [Trait("Produtoservice", "ProductoCreateHandler")]
    public async Task Test2()
    {
        // Arrange

        var command = new CreateProductCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.False(result.Succeeded);
        _productRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Product>()), Times.Never);
    }

}