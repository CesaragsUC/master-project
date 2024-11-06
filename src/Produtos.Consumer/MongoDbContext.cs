using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Produtos.Consumer;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        // Configura a representação global de Guid para evitar problemas de compatibilidade
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

    public IMongoDatabase Database => _database;
}

// Usando Studio 3T para criar um usuário no MongoDB
//use admin;
//db.getUsers();


//db.createUser({
//  user: "cesar",
//  pwd: "cesar",
//  roles: [{ role: "root", db: "admin" }]
//});

//db.createUser({
//  user: "cesar",
//  pwd: "cesar",
//  roles: [
//    { role: "readWrite", db: "MediatrDemo" },
//    { role: "readWrite", db: "admin" }
//  ]
//});