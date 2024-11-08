using Api.Catalogo.Abstractions;
using Api.Catalogo.Models;
using Api.Catalogo.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Api.Catalogo.Tests;

public class MongoRepositoryTest
{

    private readonly Mock<IMongoDbContext> _mockContext;
    private readonly Mock<IMongoCollection<Product>> _mockCollection;
    private readonly MongoRepository<Product> _mongoRepository;
    public MongoRepositoryTest()
    {
        _mockCollection = new Mock<IMongoCollection<Product>>();

        _mockContext = new Mock<IMongoDbContext>();

        _mockContext
            .Setup(c => c.GetCollection<Product>(It.IsAny<string>()))
            .Returns(_mockCollection.Object);

        
        _mongoRepository = new MongoRepository<Product>(_mockContext.Object);

        //// Descomentar caso queira usar o método InitializeMongoRepository
        //var (repository, collection) = BaseConfig.InitializeMongoRepository<Produtos>();
        //_mongoRepository = repository;
        //_mockCollection = collection;
    }


    [Fact(DisplayName = "Teste 01 - Deve inserir produto com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste01()
    {
        // Arrange
        var produto = ProductFactoryTests.CriarProduto();

        // Act
        await _mongoRepository.Insert(produto);

        // Assert
        _mockCollection.Verify(c => c.InsertOneAsync(It.IsAny<Product>(), null, default), Times.Once);
    }

    [Fact(DisplayName = "Teste 02 - Deve inserir lista produto com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste02()
    {
        // Arrange
        var produtos = ProductFactoryTests.CriarProdutoLista();

        // Act
        await _mongoRepository.InsertMany(produtos);

        // Assert
        _mockCollection.Verify(c => c.InsertManyAsync(It.IsAny<List<Product>>(), null, default), Times.Once);
    }

    [Fact(DisplayName = "Teste 04 - Deve obter produto pelo ID com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste03()
    {
        // Arrange
        var produto = ProductFactoryTests.CriarProduto();

        // Opcao alternativa ao invez de usar os dados do construtor
        var (repository, collection) = BaseConfig.InitializeMongoRepository<Product>();

        // Mock do retorno do Find para retornar um cursor mockado
        var mockCursor = new Mock<IAsyncCursor<Product>>();

        mockCursor.Setup(c => c.Current).Returns(new List<Product> { produto });

        mockCursor
            .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true);

        mockCursor
            .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);


        collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                    It.IsAny<FindOptions<Product>>(),
                                    It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(mockCursor.Object); // Retorna o cursor mockado


        // Act

        var result = await repository.GetById(nameof(produto.ProductId), Guid.Parse(produto.ProductId!));


        // Assert

        collection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                       It.IsAny<FindOptions<Product>>(),
                                       It.IsAny<CancellationToken>()),
                                       Times.Once);

        Assert.Equal(produto._id, result._id);  // Verifica se o ID retornado é o esperado
    }

    [Fact(DisplayName = "Teste 04 - Deve obter produto pelo Nome com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste04()
    {
        // Arrange
        var produtos = ProductFactoryTests.CriarProdutoLista();

        var produto = produtos.FirstOrDefault();

        // Opcao alternativa ao invez de usar os dados do construtor
        var (repository, collection) = BaseConfig.InitializeMongoRepository<Product>();

        var filter = Builders<Product>.Filter.Regex(nameof(produto.Name), new BsonRegularExpression(produto?.Name!, "i"));

        // Mock do retorno do Find para retornar um cursor mockado
        var mockCursor = new Mock<IAsyncCursor<Product>>();

        mockCursor.Setup(c => c.Current).Returns(produtos);

        mockCursor
            .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)  // Primeira chamada para MoveNext retorna true
            .Returns(false); // Segunda chamada para MoveNext retorna false

        mockCursor
            .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)  // Primeira chamada para MoveNextAsync retorna true
            .ReturnsAsync(false); // Segunda chamada para MoveNextAsync retorna false

        collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                    It.IsAny<FindOptions<Product>>(),
                                    It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(mockCursor.Object); // Retorna o cursor mockado


        // Act

        var result = await repository.GetByName(nameof(produto.Name), produto?.Name!);
        // Assert

        collection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                       It.IsAny<FindOptions<Product>>(),
                                       It.IsAny<CancellationToken>()),
                                       Times.Once);

        Assert.Equal(produto!.Name, result!.Items.FirstOrDefault()!.Name);
        Assert.True(result!.Items.Count > 0);
    }

    [Fact(DisplayName = "Teste 05 - Deve obter todos os produtos com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste05()
    {
        // Arrange
        var produtos = ProductFactoryTests.CriarProdutoLista();

        var produto = produtos.FirstOrDefault();

        // Opcao alternativa ao invez de usar os dados do construtor
        var (repository, collection) = BaseConfig.InitializeMongoRepository<Product>();


        // Mock do retorno do Find para retornar um cursor mockado
        var mockCursor = new Mock<IAsyncCursor<Product>>();

        mockCursor.Setup(c => c.Current).Returns(produtos);

        mockCursor
            .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)  // Primeira chamada para MoveNext retorna true
            .Returns(false); // Segunda chamada para MoveNext retorna false

        mockCursor
            .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)  // Primeira chamada para MoveNextAsync retorna true
            .ReturnsAsync(false); // Segunda chamada para MoveNextAsync retorna false


        collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                    It.IsAny<FindOptions<Product>>(),
                                    It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(mockCursor.Object); // Retorna o cursor mockado

        var result = await repository.GetAll();

        Assert.True(result.Items.Count > 0);
        Assert.True(result != null);
    }
}

