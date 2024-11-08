using MongoDB.Driver;

namespace Catalog.Services.Abstractions;

public interface IQueryFilter
{
    SortDefinition<TEntity> GetSortDefinition<TEntity>();
}
