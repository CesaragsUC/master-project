﻿using MongoDB.Driver;

namespace Api.Catalogo.Abstractions;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
    IMongoDatabase Database  { get; }
}