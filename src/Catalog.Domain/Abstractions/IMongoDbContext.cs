using MongoDB.Driver;

namespace Catalog.Domain.Abstractions;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
    IMongoDatabase Database { get; }
}