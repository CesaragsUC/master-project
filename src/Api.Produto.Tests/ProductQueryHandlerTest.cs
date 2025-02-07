using MediatR;
using Moq;
using Product.Application.Handlers.Product;
using Product.Application.Queries.Product;
using RepoPgNet;
using System.Linq.Expressions;

namespace Product.Api.Tests;

public class ProductQueryHandlerTest : BaseConfig
{

    private readonly Mock<IPgRepository<Domain.Models.Product>> _repository;
    private readonly Mock<IMediator> _mediator;
    private ProdutoQueryHandler _handler;
    public ProductQueryHandlerTest()
    {
        InitializeMediatrService();

        _repository = new Mock<IPgRepository<Domain.Models.Product>>();
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
                       return predicate.Compile().Invoke(product) ? product : null;
                   });


        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.NotNull(result);
    }

}
