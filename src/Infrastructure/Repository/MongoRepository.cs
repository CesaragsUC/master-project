using Domain.Interfaces;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Infrastructure.Repository
{
    [ExcludeFromCodeCoverage]
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
    {
        private readonly IMongoDbContext _dbContext;

        public MongoRepository(IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(TEntity obj, string collectionName)
        {
            var collection = _dbContext.GetCollection<TEntity>(collectionName);
            await collection.InsertOneAsync(obj);
        }

        public async Task UpdateAsync(string field, TEntity obj, string collectionName)
        {
            var filter = Builders<TEntity>.Filter.Eq(field, GetEntityId(field, obj));

            var update = GetObjectProperties(obj);

            var collection = _dbContext.GetCollection<TEntity>(collectionName);
            await collection.UpdateOneAsync(filter, update);
        }

        public async Task Delete(string field, Guid id, string collectionName)
        {
            var filter = Builders<TEntity>.Filter.Eq(field, id);
            var collection = _dbContext.GetCollection<TEntity>(collectionName);
            await collection.DeleteOneAsync(filter);
        }

        private object GetEntityId(string field, TEntity entity)
        {
            var propertyInfo = entity.GetType().GetProperty(field);
            var value = propertyInfo?.GetValue(entity);

            // Verificar se o valor é um Guid ou uma string
            if (value is Guid guidValue)
            {
                return guidValue;
            }

            if (value is string stringValue && Guid.TryParse(stringValue, out var parsedGuid))
            {
                return parsedGuid;
            }

            // Se o valor não for Guid nem string, retornar como está (evitar erro de conversão)
            return value;
        }

        private UpdateDefinition<T> GetObjectProperties<T>(T obj)
        {
            var updateDefinition = Builders<T>.Update.Combine();

            Type tipo = obj.GetType();

            foreach (PropertyInfo propriedade in tipo.GetProperties())
            {
                string nomePropriedade = propriedade.Name;

                if (nomePropriedade == "_id")
                {
                    continue; 
                }

                object valorPropriedade = propriedade.GetValue(obj);

                if (valorPropriedade != null)
                {
                    updateDefinition = updateDefinition.Set(nomePropriedade, valorPropriedade);
                }
            }

            return updateDefinition;
        }
    }
}
