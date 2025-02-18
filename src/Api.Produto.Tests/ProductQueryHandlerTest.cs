using HybridRepoNet.Abstractions;
using HybridRepoNet.Repository;
using Infrastructure;
using MediatR;
using Moq;
using Product.Application.Handlers.Product;
using Product.Application.Queries.Product;
using System.Linq.Expressions;

namespace Product.Api.Tests;

public class ProductQueryHandlerTest : BaseConfig
{

    private readonly Mock<IUnitOfWork<ProductDbContext>> _unitOfWork;
    private readonly Mock<IMediator> _mediator;
    private ProdutoQueryHandler _handler;
    public ProductQueryHandlerTest()
    {
        InitializeMediatrService();

        _unitOfWork = new Mock<IUnitOfWork<ProductDbContext>>();
        _mediator = new Mock<IMediator>();
        _handler = new ProdutoQueryHandler(_unitOfWork.Object);
    }

    [Fact(DisplayName = "Teste 01- - Rotornar lista com sucesso")]
    [Trait("Produtoservice", "ProductoQueryHandler")]
    public async Task Test1()
    {
        var command = new ProductQuery();

        _unitOfWork.Setup(r => r.Repository<Domain.Models.Product>().GetAllAsync()).ReturnsAsync(() =>
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
                       return predicate.Compile().Invoke(product) ? product : null;
                   });


        var result = await _handler.Handle(command, CancellationToken.None);

        // Act
        Assert.NotNull(result);
    }

}
