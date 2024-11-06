using Api.Catalogo.Abstractions;
using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Api.Catalogo.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Api.Catalogo.Repository;

public interface IProdutoRepository 
{
    Task<PagedResult<Produtos>> GetAll(ProdutoFilter filter);
}

public class ProdutoRepository : IProdutoRepository
{
    private readonly IMongoCollection<Produtos> _mongoCollection;

    public ProdutoRepository(IMongoDbContext dbContext)
    {
        _mongoCollection = dbContext.GetCollection<Produtos>(typeof(Produtos).Name);
    }
    public async Task<PagedResult<Produtos>> GetAll(ProdutoFilter filter)
    {
        var builder = Builders<Produtos>.Filter;
        var filterDefinition = builder.Empty;

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            filterDefinition &= builder.Regex("Nome", new BsonRegularExpression(filter.Name, "i"));
        }

        if (filter.MinPrice.HasValue)
        {
            filterDefinition &= builder.Gte("Preco", filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            filterDefinition &= builder.Lte("Preco", filter.MaxPrice.Value);
        }

        if (filter.OnlyActive.HasValue)
        {
            filterDefinition &= builder.Eq("Active", filter.OnlyActive.Value);
        }

        var results = await _mongoCollection
            .Find(filterDefinition)
            .Sort(filter.GetSortDefinition<Produtos>())
            .Skip((filter.Page - 1) * filter.PageSize)
            .Limit(filter.PageSize)
            .ToListAsync();

        var totalItems = await _mongoCollection.CountDocumentsAsync(filterDefinition);

        return new PagedResult<Produtos>
        {
            Items = results,
            TotalCount = (int)totalItems,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
}
