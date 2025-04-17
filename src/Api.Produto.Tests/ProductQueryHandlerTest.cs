using HybridRepoNet.Repository;
using MediatR;
using Moq;
using Product.Application.Handlers.Product;
using Product.Application.Queries.Product;
using Product.Domain.Abstractions;
using System.Linq.Expressions;

namespace Product.Api.Tests;

public class ProductQueryHandlerTest : BaseConfig
{

    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<IMediator> _mediator;
    private ProdutoQueryHandler _handler;
    public ProductQueryHandlerTest()
    {
        InitializeMediatrService();

        _productRepository = new Mock<IProductRepository>();
        _mediator = new Mock<IMediator>();
        _handler = new ProdutoQueryHandler(_productRepository.Object);
    }

    [Fact(DisplayName = "Teste 01- - Rotornar lista com sucesso")]
    [Trait("Produtoservice", "ProductoQueryHandler")]
    public async Task Test1()
    {
        var command = new ProductQuery();

        _productRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(() =>
        {
            return new List<Domain.Models.Product>
            {
                new Domain.Models.Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Produtos 01",
                    Price = 10.5m,
                    Active = true,
                    CreatAt = DateTime.Now
                }
            };
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.True(result.Any());

    }

    [Fact(DisplayName = "Teste 02 - Obter por Id com sucesso")]
    [Trait("Produtoservice", "ProductoQueryHandler")]
    public async Task Test2()
    {
        var command = new ProductByIdQuery { Id = Guid.NewGuid() };

        var product = new Domain.Models.Product
        {
            Id = command.Id,
            Name = "Produtos Teste",
            Price = 20.00m,
            Active = true,
            CreatAt = DateTime.Now
        };

        _productRepository.Setup(r => r.FindOne(It.IsAny<Guid>())).Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.NotNull(result);
    }

}
