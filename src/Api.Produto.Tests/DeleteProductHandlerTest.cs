using HybridRepoNet.Repository;
using Moq;
using Product.Application.Comands.Product;
using Product.Application.Handlers.Product;
using Product.Domain.Abstractions;
using Product.Domain.Models;
using System.Linq.Expressions;


namespace Product.Api.Tests;

public class DeleteProductHandlerTest : BaseConfig
{

    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<IProductService> _productService;
    private readonly DeleteProductHandler _handler;
    public DeleteProductHandlerTest()
    {
        InitializeMediatrService();

        _productRepository = new Mock<IProductRepository>();
        _productService = new Mock<IProductService>();

        _handler = new DeleteProductHandler(_productService.Object, _productRepository.Object);
    }


    [Fact(DisplayName = "Teste 01 - Deletar com sucesso")]
    [Trait("Produtoservice", "ProductDeleteHandler")]
    public async Task Test1()
    {
        var command = new DeleteProductCommand(Guid.NewGuid());

        var product = new Domain.Models.Product
        {
            Id = command.Id,
            Name = "Produtos Teste",
            Price = 20.00m,
            Active = true,
            CreatAt = DateTime.Now
        };

        _productRepository.Setup(r => r.FindOne(It.IsAny<Guid>())).Returns(product);


        _productRepository.Setup(r => r.Delete(It.IsAny<Domain.Models.Product>())).Callback<Domain.Models.Product>((product) =>
        {
            // Se o produto for válido, retorne o produto
            if (product != null)
            {
                return;
            }
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.True(result.Succeeded);
        _productRepository.Verify(r => r.FindOne(It.IsAny<Guid>()), Times.Once);
        _productRepository.Verify(r => r.Delete(It.IsAny<Domain.Models.Product>()), Times.Once);
    }

    [Fact(DisplayName = "Teste 02 - Deletar com erro")]
    [Trait("Produtoservice", " ProductDeleteHandler")]
    public async Task Test2()
    {
        // Arrange

        var command = new DeleteProductCommand(Guid.Empty);


        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.False(result.Succeeded);
        _productRepository.Verify(r => r.FindOne(It.IsAny<Guid>()), Times.Never);
        _productRepository.Verify(r => r.Delete(It.IsAny<Product.Domain.Models.Product>()), Times.Never);
    }
}