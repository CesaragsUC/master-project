using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Api.Catalogo.Abstractions;

namespace Api.Catalogo.DbContext;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    // Variável para garantir o registro único
    private bool _guidSerializerRegistered = false;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        // Registra o serializador de Guid apenas uma vez
        if (!_guidSerializerRegistered)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            _guidSerializerRegistered = true; // Marca como registrado
        }

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