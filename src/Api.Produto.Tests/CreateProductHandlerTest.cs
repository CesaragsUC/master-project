using Domain.Interfaces;
using HybridRepoNet.Abstractions;
using Infrastructure;
using Moq;
using Product.Application.Comands.Product;
using Product.Application.Handlers.Product;
using Product.Domain.Abstractions;

//https://goatreview-com.cdn.ampproject.org/c/s/goatreview.com/mediatr-quickly-test-handlers-with-unit-tests/amp/

namespace Product.Api.Tests;

public class CreateProductHandlerTest : BaseConfig
{
    private readonly Mock<IUnitOfWork<ProductDbContext>> _unitOfWork;
    private readonly Mock<IProductService> _productService;
    private readonly Mock<IBobStorageService> _bobStorageService;
    private readonly CreateProductHandler _handler;
    public CreateProductHandlerTest()
    {
        InitializeMediatrService();

        _unitOfWork = new Mock<IUnitOfWork<ProductDbContext>>();
        _productService = new Mock<IProductService>();
        _bobStorageService = new Mock<IBobStorageService>();
        _handler = new CreateProductHandler(_bobStorageService.Object, _productService.Object, _unitOfWork.Object);
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

        _unitOfWork.Setup(r => r.Repository<Domain.Models.Product>().AddAsync(It.IsAny<Domain.Models.Product>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.True(result.Succeeded);

        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().AddAsync(It.IsAny<Domain.Models.Product>()), Times.Once);
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
        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().AddAsync(It.IsAny<Domain.Models.Product>()), Times.Never);
    }
}