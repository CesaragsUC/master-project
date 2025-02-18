using Domain.Interfaces;
using HybridRepoNet.Abstractions;
using HybridRepoNet.Repository;
using Infrastructure;
using Moq;
using Product.Application.Comands.Product;
using Product.Application.Handlers.Product;
using Product.Domain.Abstractions;
using System.Linq.Expressions;


namespace Product.Api.Tests;

public class AtualizarProdutoHandlerTest : BaseConfig
{

    private readonly Mock<IUnitOfWork<ProductDbContext>> _unitOfWork;
    private readonly Mock<IBobStorageService> _bobStorageService;
    private readonly Mock<IProductService> _productService;
    private readonly UpdateProductHandler _handler;
    public AtualizarProdutoHandlerTest()
    {
        InitializeMediatrService();

        _productService = new Mock<IProductService>();
        _bobStorageService = new Mock<IBobStorageService>();
        _unitOfWork = new Mock<IUnitOfWork<ProductDbContext>>();

        _handler = new UpdateProductHandler(_bobStorageService.Object, _productService.Object, _unitOfWork.Object);
    }

    [Fact(DisplayName = "Teste 01 - Atualizar com sucesso")]
    [Trait("Produtoservice", " ProductUpdateHandler")]
    public async Task Test1()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            Id =  Guid.NewGuid(),
            Name = "Produtos 01",
            Price = 10.5m,
            Active = true
        };

        // Configura o callback para o método FindOne
        _unitOfWork.Setup(r => r.Repository<Domain.Models.Product>().FindOne(It.IsAny<Expression<Func<Domain.Models.Product, bool>>>(), null))
                   .Callback<Expression<Func<Domain.Models.Product, bool>>, FindOptions?>((predicate, options) =>
                   { })
                   .Returns<Expression<Func<Domain.Models.Product, bool>>, FindOptions?>((predicate, options) =>
                   {
                       var product = new Domain.Models.Product
                       {
                           Id = command.Id,
                           Name = "Produtos Teste",
                           Price = 20.00m,
                           Active = true,
                           CreatAt = DateTime.Now
                       };
                       // Se o predicate for válido, retorne o Produtos
                       return predicate.Compile().Invoke(product) ? product : null;
                   });


        _unitOfWork.Setup(r => r.Repository<Domain.Models.Product>().Update(It.IsAny<Domain.Models.Product>()))
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
        Assert.True(result.Succeeded);

        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().FindOne(It.IsAny<Expression<Func<Domain.Models.Product, bool>>>(), null), Times.Once);
        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().Update(It.IsAny<Domain.Models.Product>()), Times.Once);
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


        // Configura o callback para o método FindOne
        _unitOfWork.Setup(r => r.Repository<Domain.Models.Product>().FindOne(It.IsAny<Expression<Func<Domain.Models.Product, bool>>>(), null))
                   .Callback<Expression<Func<Domain.Models.Product, bool>>, FindOptions?>((predicate, options) =>
                   { })
                   .Returns<Expression<Func<Domain.Models.Product, bool>>, FindOptions?>((predicate, options) =>
                   {
                       var product = new Domain.Models.Product
                       {
                           Id = Guid.NewGuid(),
                           Name = "Produtos Teste",
                           Price = 20.00m,
                           Active = true,
                           CreatAt = DateTime.Now
                       };
                       // Se o predicate for válido, retorne o Produtos
                       return predicate.Compile().Invoke(product) ? product : null;
                   });


        _unitOfWork.Setup(r => r.Repository<Domain.Models.Product>().Update(It.IsAny<Domain.Models.Product>()))
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
        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().Update(It.IsAny<Domain.Models.Product>()), Times.Never);
    }
}