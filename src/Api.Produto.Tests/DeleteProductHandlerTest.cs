using MediatR;
using Moq;
using Product.Application.Comands.Product;
using Product.Application.Handlers.Product;
using Product.Domain.Abstractions;
using RepoPgNet;
using System.Linq.Expressions;


namespace Product.Api.Tests;

public class DeleteProductHandlerTest : BaseConfig
{

    private readonly Mock<IPgRepository<Domain.Models.Product>> _repository;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IProductService> _productService;
    private DeleteProductHandler _handler;
    public DeleteProductHandlerTest()
    {
        InitializeMediatrService();

        _repository = new Mock<IPgRepository<Domain.Models.Product>>();
        _mediator = new Mock<IMediator>();
        _productService = new Mock<IProductService>();

        _handler = new DeleteProductHandler(_repository.Object, _productService.Object);
    }


    [Fact(DisplayName = "Teste 01 - Deletar com sucesso")]
    [Trait("Produtoservice", "ProductDeleteHandler")]
    public async Task Test1()
    {
        var command = new DeleteProductCommand(Guid.NewGuid());

        // Configura o callback para o método FindOne
        _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Product.Domain.Models.Product, bool>>>(), null))
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


        _repository.Setup(r => r.DeleteAsync(It.IsAny<Domain.Models.Product>()))
           .Callback<Domain.Models.Product>(p =>
           { })
           .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.True(result.Succeeded);
        _repository.Verify(r => r.FindOne(It.IsAny<Expression<Func<Domain.Models.Product, bool>>>(), null), Times.Once);
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Domain.Models.Product>()), Times.Once);
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
        _repository.Verify(r => r.FindOne(It.IsAny<Expression<Func<Product.Domain.Models.Product, bool>>>(), null), Times.Never);
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Product.Domain.Models.Product>()), Times.Never);
    }
}