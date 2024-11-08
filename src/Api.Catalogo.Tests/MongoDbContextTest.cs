using Api.Catalogo.Abstractions;
using Api.Catalogo.DbContext;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Catalogo.Tests;

public class MongoDbContextTest
{

    private readonly IMongoDbContext _mongoDbContext;

    public MongoDbContextTest()
    {
        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "MediatrDemo"
        });

        _mongoDbContext = new MongoDbContext(settings);
    }


    [Fact(DisplayName = "Teste 01 - Deve se conectar ao banco com sucesso.")]
    [Trait("Catalogo", "MongoDbContextTest")]
    public async Task Test_AddProduto_Integration()
    {
        // Act: Pega a lista de coleções do banco de dados para verificar a conexão
        var collectionNames =  _mongoDbContext?.Database?.ListCollectionNames().ToList();

        // Assert: Verifica se a conexão com o banco foi bem-sucedida ao tentar acessar coleções
        Assert.NotNull(collectionNames);
        
    }
}