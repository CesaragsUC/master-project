using Api.Catalogo.Abstractions;
using MongoDB.Driver;

namespace Api.Catalogo.Filters;


public class ProdutoFilter : IQueryFilter
{
    public bool? OnlyActive { get; set; } = null;

    public string Name { get; set; } = string.Empty;

    public decimal? MinPrice { get; set; } = null;
    public decimal? MaxPrice { get; set; } = null;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string OrderBy { get; set; } = "CreatAt";


    public string OrderDirection { get; set; } = "asc";

    public int Skip => (Page - 1) * PageSize;

    public SortDefinition<TEntity> GetSortDefinition<TEntity>()
    {
        var sortBuilder = Builders<TEntity>.Sort;
        return OrderDirection.ToLower() switch
        {
            "desc" => sortBuilder.Descending(OrderBy),
            _ => sortBuilder.Ascending(OrderBy),
        };
    }

}

