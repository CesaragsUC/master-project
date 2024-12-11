using Api.Catalogo.Mapping;
using Application.Dtos.Dtos.Produtos;
using AutoMapper;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Models;
using Catalog.Service.Abstractions;
using Catalog.Service.Services;
using Catalog.Services.Filters;
using Catalogo.Api.Tests;
using FluentAssertions;
using MongoDB.Driver;
using Moq;

namespace Api.Catalogo.Tests
{
    public class ProdutoServiceTest
    {

        private readonly Mock<IMongoDbContext> _mockContext;
        private readonly Mock<IMongoCollection<Products>> _mockCollection;
        private readonly Mock<IMongoRepository<Products>> _mongoRepository;
        private readonly Mock<IProductRepository> _produtoRepository;
        private readonly ProductService _service;
        private readonly IMapper _mapper;
        public ProdutoServiceTest()
        {
            _mockCollection = new Mock<IMongoCollection<Products>>();

            _mongoRepository = new Mock<IMongoRepository<Products>>();

            _produtoRepository = new Mock<IProductRepository>();

            _mockContext = new Mock<IMongoDbContext>();

            _mockContext
                .Setup(c => c.GetCollection<Products>(It.IsAny<string>()))
                .Returns(_mockCollection.Object);

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

            _mongoRepository.Setup(c => c.InsertAsync(It.IsAny<Products>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Insert(produto);

            // Assert
            _mongoRepository.Verify(c => c.InsertAsync(It.IsAny<Products>()), Times.Once);


            result.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "Teste 02 - Obter produto pelo ID com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test2()
        {

            // Arrange
            var produto = ProductFactoryTests.CriarProduto();

            _mongoRepository.Setup(c => c.GetById(It.IsAny<string>(),It.IsAny<Guid>())).ReturnsAsync(produto);

            // Act
            var result = await _service.GetById(nameof(produto.ProductId), Guid.Parse(produto.ProductId!));

            // Assert
            _mongoRepository.Verify(c => c.GetById(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

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

            _mongoRepository.Setup(c => c.GetById(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync((Products)null);

            // Act
            var result = await _service.GetById(nameof(produto.ProductId), Guid.Parse(produto.ProductId!));

            // Assert
            _mongoRepository.Verify(c => c.GetById(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

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

            _mongoRepository.Setup(c => c.GetByName(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PagedResult<Products> {
                Items = produto,
                TotalCount = produto.Count
            });

            // Act
            var result = await _service.GetByName("Nome", produto!.FirstOrDefault()?.Name!);

            // Assert
            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
            result?.Data.Should().NotBeNull();
            result?.Data?.Count.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "Teste 05 - Obter produto Nome ID deve retornar erro")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test5()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoLista();

            _mongoRepository.Setup(c => c.GetByName(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PagedResult<Products>
            {
                Items = null,
                TotalCount = 0
            });

            // Act
            var result = await _service.GetByName("Nome", produto!.FirstOrDefault()?.Name!);

            // Assert
            result?.Errors.Should().Contain("Product not found");
            result?.Success.Should().BeFalse();
            result?.Data.Should().BeNull();
            result?.Data?.Count.Should().BeLessThanOrEqualTo(0);
        }

        [Fact(DisplayName = "Teste 06 - Inserir  produto com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test6()
        {
            // Arrange
            var produto = ProductFactoryTests.CriarProdutoAddDto();

            _mongoRepository.Setup(c => c.InsertAsync(It.IsAny<Products>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Insert(produto);

            // Assert
            _mongoRepository.Verify(c => c.InsertAsync(It.IsAny<Products>()), Times.Once);

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

            // Assert
            _mongoRepository.Verify(c => c.InsertAsync(It.IsAny<Products>()), Times.Never);

            result?.Errors.Should().HaveCountGreaterThan(0);
            result?.Success.Should().BeFalse();
        }

        [Fact(DisplayName = "Teste 06 - Inserir lista de produto com sucesso")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test8()
        {
            // Arrange
            var produtos = ProductFactoryTests.CriarProdutoAddDtoLista();

            _mongoRepository.Setup(c => c.InsertMany(It.IsAny<List<Products>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.InsertMany(produtos);

            // Assert
            _mongoRepository.Verify(c => c.InsertMany(It.IsAny<List<Products>>()), Times.Once);

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

            _mongoRepository.Setup(c => c.InsertMany(It.IsAny<List<Products>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.InsertMany(produtos);

            // Assert
            _mongoRepository.Verify(c => c.InsertMany(It.IsAny<List<Products>>()), Times.Never);

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

            // Act
            var result = await _service.Update(newProduto);

            // Assert
            _mongoRepository.Verify(c => c.UpdateAsync(It.IsAny<string>(), It.IsAny<Products>()), Times.Once);

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

            _mongoRepository.Setup(c => c.Delete("ProdutoId", It.IsAny<Guid>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete("ProdutoId",Guid.Parse(produto.ProductId!));

            // Assert
            _mongoRepository.Verify(c => c.Delete(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            result?.Message.Should().Contain("Produto excluido com sucesso");
            result?.Errors.Should().BeNull();
            result?.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "Teste 11 - Deletar produto invalido deve retornar erro")]
        [Trait("Catalogo", "ProdutoServiceTest")]
        public async Task Test13()
        {
            // Arrange
            _mongoRepository.Setup(c => c.Delete("ProdutoId", It.IsAny<Guid>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(string.Empty, Guid.Empty);

            // Assert
            _mongoRepository.Verify(c => c.Delete(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);

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

    }
}