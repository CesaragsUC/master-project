using MongoDB.Driver;

namespace Api.Catalogo.Abstractions;

public interface IQueryFilter
{ 
    SortDefinition<TEntity> GetSortDefinition<TEntity>();
}

