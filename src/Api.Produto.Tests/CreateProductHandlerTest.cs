using Domain.Handlers.Comands;
using Domain.Interfaces;
using Moq;
using Product.Domain.Abstractions;
using Product.Domain.Handlers;

//https://goatreview-com.cdn.ampproject.org/c/s/goatreview.com/mediatr-quickly-test-handlers-with-unit-tests/amp/

namespace Product.Api.Tests;

public class CreateProductHandlerTest : BaseConfig
{
    private readonly Mock<IRepository<Product.Domain.Models.Product>> _repository;
    private readonly Mock<IProductService> _productService;
    private readonly Mock<IBobStorageService> _bobStorageService;
    private readonly CreateProductHandler _handler;
    public CreateProductHandlerTest()
    {
        InitializeMediatrService();

        _repository = new Mock<IRepository<Product.Domain.Models.Product>>();
        _productService = new Mock<IProductService>();
        _bobStorageService = new Mock<IBobStorageService>();
        _handler = new CreateProductHandler(_repository.Object, _bobStorageService.Object, _productService.Object);
    }

    [Fact(DisplayName = "Teste 01 - Com sucesso")]
    [Trait("Produtoservice", "ProductoCreateHandler")]
    public async Task Test1()
    {
        // Arrange

        var command = new CreateProductCommand
        {
            Name = "Produtos 01",
            Price = 10.5m,
            Active = true
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.True(result.Succeeded);

        _repository.Verify(r => r.Add(It.IsAny<Product.Domain.Models.Product>()), Times.Once);
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
        _repository.Verify(r => r.Add(It.IsAny<Product.Domain.Models.Product>()), Times.Never);
    }
}