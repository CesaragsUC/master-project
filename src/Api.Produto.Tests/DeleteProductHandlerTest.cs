using HybridRepoNet.Abstractions;
using HybridRepoNet.Repository;
using Infrastructure;
using MediatR;
using Moq;
using Product.Application.Comands.Product;
using Product.Application.Handlers.Product;
using Product.Domain.Abstractions;
using System.Linq.Expressions;


namespace Product.Api.Tests;

public class DeleteProductHandlerTest : BaseConfig
{

    private readonly Mock<IUnitOfWork<ProductDbContext>> _unitOfWork;
    private readonly Mock<IProductService> _productService;
    private readonly DeleteProductHandler _handler;
    public DeleteProductHandlerTest()
    {
        InitializeMediatrService();

        _unitOfWork = new Mock<IUnitOfWork<ProductDbContext>>();
        _productService = new Mock<IProductService>();

        _handler = new DeleteProductHandler(_productService.Object, _unitOfWork.Object);
    }


    [Fact(DisplayName = "Teste 01 - Deletar com sucesso")]
    [Trait("Produtoservice", "ProductDeleteHandler")]
    public async Task Test1()
    {
        var command = new DeleteProductCommand(Guid.NewGuid());

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


        _unitOfWork.Setup(r => r.Repository<Domain.Models.Product>().Delete(It.IsAny<Domain.Models.Product>())).Callback<Domain.Models.Product>((product) =>
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
        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().Delete(It.IsAny<Domain.Models.Product>()), Times.Once);
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
        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().FindOne(It.IsAny<Expression<Func<Product.Domain.Models.Product, bool>>>(), null), Times.Never);
        _unitOfWork.Verify(r => r.Repository<Domain.Models.Product>().Delete(It.IsAny<Product.Domain.Models.Product>()), Times.Never);
    }
}