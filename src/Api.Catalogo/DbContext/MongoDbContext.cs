﻿using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Api.Catalogo.Abstractions;

namespace Api.Catalogo.DbContext;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly bool _guidSerializerRegistered = false;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        if (!_guidSerializerRegistered)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            _guidSerializerRegistered = true;
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