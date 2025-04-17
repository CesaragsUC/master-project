using NetArchTest.Rules;
using Product.Application.Services;
using Product.Infrastructure.Repository;

namespace Api.Produtos.Tests;

public class ArchitectureTest
{
    [Fact(DisplayName = "Test 01 - Infrastructure should depend on Domain.")]
    [Trait("ArchitectureTest", "Architecture Test Design")]
    public void Test1()
    {
        var result = Types.InAssembly(typeof(ProductRepository).Assembly)
            .That()
            .ResideInNamespace("Product.Infrastructure.Repository")
            .Should()
            .HaveDependencyOn("Product.Domain")
            .GetResult();

        Assert.True(result.IsSuccessful, "The Infrastructure layer depends on Domain ");
    }

    [Fact(DisplayName = "Test 02 - Domain shouldn't depend on Infrastructure or Application layer.")]
    [Trait("ArchitectureTest", "Architecture Test Design")]
    public void Test2()
    {
        var result = Types.InAssembly(typeof(Product.Domain.Models.Product).Assembly)
            .That()
            .ResideInNamespace("Product.Domain")
            .ShouldNot()
            .HaveDependencyOnAny("Product.Infrastructure", "Product.Application")
            .GetResult();

        Assert.True(result.IsSuccessful, "The Domain layer doesn't depend on Infrastructure or Application layers");
    }


}
