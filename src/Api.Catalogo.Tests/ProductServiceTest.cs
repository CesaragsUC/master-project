using Api.Catalogo.Mapping;
using AutoMapper;
using Catalog.Application.Abstractions;
using Catalog.Application.Dtos;
using Catalog.Application.Filters;
using Catalog.Application.Services;
using Catalog.Domain.Models;
using Catalogo.Api.Tests;
using EasyMongoNet.Abstractions;
using EasyMongoNet.Utils;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;

namespace Api.Catalogo.Tests
{
    public class ProdutoServiceTest
    {

        private readonly Mock<IMongoRepository<Products>> _mongoRepository;
        private readonly Mock<IProductRepository> _produtoRepository;
        private readonly ProductService _service;
        private readonly IMapper _mapper;
        public ProdutoServiceTest()
        {

            _mongoRepository = new Mock<IMongoRepository<Products>>();

            _produtoRepository = new Mock<IProductRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperConfig());
            });

            _mapper = config.CreateMapper();

            _service = new ProductService(_mongoRepository.Object, _produtoRepository.Object, _mapper);
        }


        [Fact(DisplayName = "Teste 01 - Obter todos produtos com sucesso")]
        [Trait("Catalogo","ProdutoServiceTest")]
        public async Task Test1()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoAddDto();

            _mongoRepository.Setup(c => c.InsertOneAsync(It.IsAny<Products>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Insert(produto);

            result.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "Teste 02 - Obter produto pelo ID com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test2()
        {

            // Arrange
            var produto = ProductFactoryTests.CriarProduto();

            _mongoRepository.Setup(c => c.FindOneAsync(It.IsAny<Expression<Func<Products,bool>>>())).ReturnsAsync(produto);

            // Act
            var result = await _service.GetById(nameof(produto.ProductId), Guid.Parse(produto.ProductId!));

            result?.Data?.ProductId.Should().Be(produto.ProductId);
            result?.Success.Should().BeTrue();
            result?.Data.Should().NotBeNull();
        }

        [Fact(DisplayName = "Teste 03 - Obter produto pelo ID deve retornar erro")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test3()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProduto();

            _mongoRepository.Setup(c => c.FindOneAsync(Products => Products.ProductId == produto.ProductId)).ReturnsAsync(produto);

            // Act
            var result = await _service.GetById(nameof(produto.ProductId), Guid.Parse(produto.ProductId!));

            result?.Errors.Should().Contain("Product not found");
            result?.Success.Should().BeFalse();
            result?.Data.Should().BeNull();
        }


        [Fact(DisplayName = "Teste 04 - Obter produto pelo Nome com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test4()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoLista();

            // mock to return list of products
            _mongoRepository.Setup(c => c.FilterBy(It.IsAny<Expression<Func<Products, bool>>>())).ReturnsAsync(produto);

            // Act
            var result = await _service.GetByName(produto!.FirstOrDefault()?.Name!);

            // Assert
            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
            result?.Data.Should().NotBeNull();
            result?.Data?.Count.Should().BeGreaterThan(0);
        }


        [Fact(DisplayName = "Teste 06 - Inserir  produto com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test6()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoAddDto();

            _mongoRepository.Setup(c => c.InsertOneAsync(It.IsAny<Products>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Insert(produto);


            result?.Message.Should().Contain("Produto inserido com sucesso");
            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "Teste 7 - Inserir  produto invalido deve retornar erro")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test7()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoAddDtoInvalido();

            // Act
            var result = await _service.Insert(produto);

            result?.Errors.Should().HaveCountGreaterThan(0);
            result?.Success.Should().BeFalse();
        }

        [Fact(DisplayName = "Teste 06 - Inserir lista de produto com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test8()
        {
            // Arrange
            var produtos = ProductFactoryTests.CriarProdutoAddDtoLista();

            _mongoRepository.Setup(c => c.InsertManyAsync(It.IsAny<List<Products>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.InsertMany(produtos);


            result?.Message.Should().Contain("Produtos inserido com sucesso");
            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "Teste 7 - Inserir  lista de produto invalido deve retornar erro")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test9()
        {
            // Arrange
            var produtos = ProductFactoryTests.CriarProdutoAddDtoLista();

            var produtoInvalido = ProductFactoryTests.CriarProdutoAddDtoInvalido();
            produtos.Add(produtoInvalido);

            _mongoRepository.Setup(c => c.InsertManyAsync(It.IsAny<List<Products>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.InsertMany(produtos);

            result?.Errors.Should().HaveCountGreaterThan(0);
            result?.Success.Should().BeFalse();
        }

        [Fact(DisplayName = "Teste 08 - Deve Atualizar  produto com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test10()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoAddDto();

            var newProduto = new ProductUpdateDto
            {
                ProductId = Guid.NewGuid().ToString(),
                Name = "HeadSet Logitec",
                Price = 850.0m,
                Active = produto.Active
            };

            _mongoRepository.Setup(c => c.UpdateAsync(It.IsAny<string>(), It.IsAny<Products>())).Returns(Task.CompletedTask);
            _mongoRepository.Setup(c => c.FindByIdAsync(It.IsAny<Expression<Func<Products, bool>>>())).ReturnsAsync(ProductFactoryTests.CriarProduto());

            // Act
            var result = await _service.Update(newProduto);

            result?.Message.Should().Contain("Produtos atualizado com sucesso");
            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "Teste 8 - Atualizar produto invalido deve retornar erro")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test11()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoAddDto();

            var produtoAtualizado = new ProductUpdateDto
            {
                Name = string.Empty,
                Price = 0,
                Active = produto.Active
            };

            _mongoRepository.Setup(c => c.UpdateAsync(It.IsAny<string>(), It.IsAny<Products>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(produtoAtualizado);

            // Assert
            _mongoRepository.Verify(c => c.UpdateAsync(It.IsAny<string>(), It.IsAny<Products>()), Times.Never);

            result?.Errors.Should().HaveCountGreaterThan(0);
            result?.Success.Should().BeFalse();
        }

        [Fact(DisplayName = "Teste 10 - Deve Deletar  produto com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test12()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProduto();

            // Act
            var result = await _service.Delete("ProdutoId",Guid.Parse(produto.ProductId!));


            result?.Message.Should().Contain("Produto excluido com sucesso");
            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "Teste 11 - Deletar produto invalido deve retornar erro")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test13()
        {
            // Act
            var result = await _service.Delete(string.Empty, Guid.Empty);

            result?.Errors.Should().HaveCountGreaterThan(0);
            result?.Success.Should().BeFalse();
        }

        [Fact(DisplayName = "Teste 12 - Filtrar produto deve retornar pelomenos 1 produto com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test14()
        {
            // Arrange
            var produtoLista = ProductFactoryTests.CriarProdutoLista();

            var produto = produtoLista.FirstOrDefault(x=> x.Price > 10);

            var filters = new ProductFilter
            {
                OnlyActive = produto.Active,
                Name = produto.Name,
                MinPrice = 10,
                MaxPrice = 100,
                Page = 1,
                PageSize = 5,
                OrderBy = "CreatAt",
                OrderDirection = "asc"
            };

            _produtoRepository.Setup(c => c.GetAll(It.IsAny<ProductFilter>())).ReturnsAsync(
                new PagedResult<Products> 
                {
                    Items = produtoLista,
                    TotalCount = produtoLista.Count
                });

            // Act
            var result = await _service.GetAll(filters);

            // Assert
            _produtoRepository.Verify(c => c.GetAll(It.IsAny<ProductFilter>()), Times.Once);

            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
            result?.Data.Should().NotBeNull();
            result?.Data?.Count.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "Teste 13 - Filtrar produto invalido filtro deve retornar vazio")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test15()
        {
            // Arrange
            _produtoRepository.Setup(c => c.GetAll(It.IsAny<ProductFilter>())).ReturnsAsync(
                new PagedResult<Products>
                {
                    Items = null,
                    TotalCount = 0
                });

            // Act
            var result = await _service.GetAll(null);

            // Assert
            _produtoRepository.Verify(c => c.GetAll(It.IsAny<ProductFilter>()), Times.Never);

            result?.Errors.Should().HaveCountGreaterThan(0);
            result?.Errors.Should().Contain("Invalid filter");
            result?.Success.Should().BeFalse();
            result?.Data.Should().BeNull();
            result?.Data?.Count.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "Teste 14 - update product not founded")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Update_ProductNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var productUpdateDto = new ProductUpdateDto { 
                ProductId = Guid.NewGuid().ToString(),
                Name = "HeadSet Logitec",
                Price = 850.0m,
                Active = true

            };

            _mongoRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<Expression<Func<Products, bool>>>()))
                           .ReturnsAsync((Products)null);

            // Act
            var result = await _service.Update(productUpdateDto);

            // Assert
            Assert.False(result.Success);

        }

        [Fact(DisplayName = "Teste 15 - return a empety list")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task GetByName_ShouldReturnProducts_WhenProductsExist()
        {
            // Arrange
            var productName = "TestProduct";
            var products = new List<Products>();

            _mongoRepository.Setup(repo => repo.FilterBy(It.IsAny<Expression<Func<Products, bool>>>()))
                .ReturnsAsync(products);

            // Act
            var result = await _service.GetByName(productName);

            // Assert
            Assert.False(result.Success);

        }
    }
}