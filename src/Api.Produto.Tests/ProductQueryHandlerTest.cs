using Domain.Handlers;
using Domain.Handlers.Comands;
using Domain.Handlers.Queries;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Moq;
using System.Linq.Expressions;

namespace Tests;

public class ProductQueryHandlerTest : BaseConfig
{

    private readonly Mock<IRepository<Product>> _repository;
    private readonly Mock<IMediator> _mediator;
    private ProdutoQueryHandler _handler;
    public ProductQueryHandlerTest()
    {
        InitializeMediatrService();

        _repository = new Mock<IRepository<Product>>();
        _mediator = new Mock<IMediator>();
        _handler = new ProdutoQueryHandler(_repository.Object);
    }

    [Fact(DisplayName = "Teste 01- - Rotornar lista com sucesso")]
    [Trait("Produtoservice", "ProductoQueryHandler")]
    public async Task Test1()
    {
        var command = new ProductQuery();

        _repository.Setup(r => r.GetAll(It.IsAny<FindOptions>()))
                   .Callback<FindOptions>(p =>
                   { }).Returns(ProductFactory.CreateProductList().AsQueryable());


        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.True(result.Any());
        Assert.True(result.Count() > 0, "A lista não possui mais do que 1 item.");

    }

    [Fact(DisplayName = "Teste 02 - Obter por Id com sucesso")]
    [Trait("Produtoservice", "ProductoQueryHandler")]
    public async Task Test2()
    {
        var command = new ProductByIdQuery { Id = Guid.NewGuid() };

        _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Product, bool>>>(), null))
                   .Callback<Expression<Func<Product, bool>>, FindOptions?>((predicate, options) =>
                   { })
                   .Returns<Expression<Func<Product, bool>>, FindOptions?>((predicate, options) =>
                   {
                       var product = new Product
                       {
                           Id = command.Id,
                           Name = "Produtos Teste",
                           Price = 20.00m,
                           Active = true,
                           CreatAt = DateTime.Now
                       };
                       return predicate.Compile().Invoke(product) ? product : null;
                   });


        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.NotNull(result);
    }



}
