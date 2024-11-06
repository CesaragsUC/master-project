using Api.Catalogo.Abstractions;
using Api.Catalogo.Filters;
using Api.Catalogo.Models;
using Api.Catalogo.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Api.Catalogo.Tests;

public class MongoRepositoryTest
{

    private readonly Mock<IMongoDbContext> _mockContext;
    private readonly Mock<IMongoCollection<Produtos>> _mockCollection;
    private readonly MongoRepository<Produtos> _mongoRepository;
    public MongoRepositoryTest()
    {
        _mockCollection = new Mock<IMongoCollection<Produtos>>();

        _mockContext = new Mock<IMongoDbContext>();

        _mockContext
            .Setup(c => c.GetCollection<Produtos>(It.IsAny<string>()))
            .Returns(_mockCollection.Object);

        
        _mongoRepository = new MongoRepository<Produtos>(_mockContext.Object);

        // Descomentar caso queira usar o método InitializeMongoRepository
        //var (repository, collection) = BaseConfig.InitializeMongoRepository<Produtos>();
        //_mongoRepository = repository;
        //_mockCollection = collection;
    }


    [Fact(DisplayName = "Teste 01 - Deve inserir produto com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste01()
    {
        // Arrange
        var produto = ProdutosFactoryTests.CriarProduto();

        // Act
        await _mongoRepository.Insert(produto);

        // Assert
        _mockCollection.Verify(c => c.InsertOneAsync(It.IsAny<Produtos>(), null, default), Times.Once);
    }

    [Fact(DisplayName = "Teste 02 - Deve inserir lista produto com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste02()
    {
        // Arrange
        var produtos = ProdutosFactoryTests.CriarProdutoLista();

        // Act
        await _mongoRepository.InsertMany(produtos);

        // Assert
        _mockCollection.Verify(c => c.InsertManyAsync(It.IsAny<List<Produtos>>(), null, default), Times.Once);
    }

    [Fact(DisplayName = "Teste 04 - Deve obter produto pelo ID com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste03()
    {
        // Arrange
        var produto = ProdutosFactoryTests.CriarProduto();

        // Opcao alternativa ao invez de usar os dados do construtor
        var (repository, collection) = BaseConfig.InitializeMongoRepository<Produtos>();

        // Mock do retorno do Find para retornar um cursor mockado
        var mockCursor = new Mock<IAsyncCursor<Produtos>>();

        mockCursor.Setup(c => c.Current).Returns(new List<Produtos> { produto });

        mockCursor
            .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true);

        mockCursor
            .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);


        collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Produtos>>(),
                                    It.IsAny<FindOptions<Produtos>>(),
                                    It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(mockCursor.Object); // Retorna o cursor mockado


        // Act

        var result = await repository.GetById(nameof(produto.ProdutoId), Guid.Parse(produto.ProdutoId!));


        // Assert

        collection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<Produtos>>(),
                                       It.IsAny<FindOptions<Produtos>>(),
                                       It.IsAny<CancellationToken>()),
                                       Times.Once);

        Assert.Equal(produto._id, result._id);  // Verifica se o ID retornado é o esperado
    }

    [Fact(DisplayName = "Teste 04 - Deve obter produto pelo Nome com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste04()
    {
        // Arrange
        var produtos = ProdutosFactoryTests.CriarProdutoLista();

        var produto = produtos.FirstOrDefault();

        // Opcao alternativa ao invez de usar os dados do construtor
        var (repository, collection) = BaseConfig.InitializeMongoRepository<Produtos>();

        var filter = Builders<Produtos>.Filter.Regex(nameof(produto.Nome), new BsonRegularExpression(produto?.Nome!, "i"));

        // Mock do retorno do Find para retornar um cursor mockado
        var mockCursor = new Mock<IAsyncCursor<Produtos>>();

        mockCursor.Setup(c => c.Current).Returns(produtos);

        mockCursor
            .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)  // Primeira chamada para MoveNext retorna true
            .Returns(false); // Segunda chamada para MoveNext retorna false

        mockCursor
            .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)  // Primeira chamada para MoveNextAsync retorna true
            .ReturnsAsync(false); // Segunda chamada para MoveNextAsync retorna false

        collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Produtos>>(),
                                    It.IsAny<FindOptions<Produtos>>(),
                                    It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(mockCursor.Object); // Retorna o cursor mockado


        // Act

        var result = await repository.GetByName(nameof(produto.Nome), produto?.Nome!);
        // Assert

        collection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<Produtos>>(),
                                       It.IsAny<FindOptions<Produtos>>(),
                                       It.IsAny<CancellationToken>()),
                                       Times.Once);

        Assert.Equal(produto!.Nome, result!.Items.FirstOrDefault()!.Nome);
        Assert.True(result!.Items.Count > 0);
    }

    [Fact(DisplayName = "Teste 05 - Deve obter todos os produtos com sucesso.")]
    [Trait("Catalogo", "MongoRepository")]
    public async Task Teste05()
    {
        // Arrange
        var produtos = ProdutosFactoryTests.CriarProdutoLista();

        var produto = produtos.FirstOrDefault();

        // Opcao alternativa ao invez de usar os dados do construtor
        var (repository, collection) = BaseConfig.InitializeMongoRepository<Produtos>();


        // Mock do retorno do Find para retornar um cursor mockado
        var mockCursor = new Mock<IAsyncCursor<Produtos>>();

        mockCursor.Setup(c => c.Current).Returns(produtos);

        mockCursor
            .SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)  // Primeira chamada para MoveNext retorna true
            .Returns(false); // Segunda chamada para MoveNext retorna false

        mockCursor
            .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)  // Primeira chamada para MoveNextAsync retorna true
            .ReturnsAsync(false); // Segunda chamada para MoveNextAsync retorna false


        collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Produtos>>(),
                                    It.IsAny<FindOptions<Produtos>>(),
                                    It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(mockCursor.Object); // Retorna o cursor mockado

        var result = await repository.GetAll();

        Assert.True(result.Items.Count > 0);
        Assert.True(result != null);
    }
}

