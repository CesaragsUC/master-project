using MongoDB.Driver;
using System.Linq.Expressions;

namespace Api.Catalogo.Abstractions;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
    IMongoDatabase Database  { get; }
}