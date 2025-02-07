using MongoDB.Driver;

namespace Domain.Interfaces;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
    IMongoDatabase Database { get; }
}