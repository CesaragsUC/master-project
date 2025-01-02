using Domain.Handlers.Comands;
using Domain.Interfaces;
using Moq;
using Product.Domain.Abstractions;
using Product.Domain.Handlers;
using System.Linq.Expressions;


namespace Product.Api.Tests;

public class AtualizarProdutoHandlerTest : BaseConfig
{

    private readonly Mock<IRepository<Domain.Models.Product>> _repository;
    private readonly Mock<IBobStorageService> _bobStorageService;
    private readonly Mock<IProductService> _productService;
    private readonly UpdateProductHandler _handler;
    public AtualizarProdutoHandlerTest()
    {
        InitializeMediatrService();

        _productService = new Mock<IProductService>();
        _bobStorageService = new Mock<IBobStorageService>();
        _repository = new Mock<IRepository<Domain.Models.Product>>();

        _handler = new UpdateProductHandler(_repository.Object, _bobStorageService.Object, _productService.Object);
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
        _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Domain.Models.Product, bool>>>(), null))
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


        _repository.Setup(r => r.Update(It.IsAny<Domain.Models.Product>()))
           .Callback<Domain.Models.Product>(p =>
           { })
           .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.True(result.Succeeded);

        _repository.Verify(r => r.FindOne(It.IsAny<Expression<Func<Domain.Models.Product, bool>>>(), null), Times.Once);
        _repository.Verify(r => r.Update(It.IsAny<Domain.Models.Product>()), Times.Once);
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
        _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Domain.Models.Product, bool>>>(), null))
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


        _repository.Setup(r => r.Update(It.IsAny<Domain.Models.Product>()))
           .Callback<Domain.Models.Product>(p =>
           { })
           .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.False(result.Succeeded);
        _repository.Verify(r => r.Update(It.IsAny<Domain.Models.Product>()), Times.Never);
    }
}