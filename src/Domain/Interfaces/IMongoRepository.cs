using System.Linq.Expressions;

namespace Domain.Interfaces;

public interface IMongoRepository<TEntity> where TEntity : class
{
    Task InsertAsync(TEntity obj, string collectionName);

    /// <summary>
    ///  Atualiza Multiplos campos
    /// </summary>
    /// <param name="whereCondition"></param>
    /// <param name="updates"></param>
    /// <returns></returns>
    Task UpdateAsync(string field, TEntity obj, string collectionName);

    Task Delete(string field, Guid id, string collectionName);
}
