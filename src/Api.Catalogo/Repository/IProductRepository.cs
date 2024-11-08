using Api.Catalogo.Abstractions;
using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Api.Catalogo.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Api.Catalogo.Repository;

public interface IProductRepository 
{
    Task<PagedResult<Product>> GetAll(ProductFilter filter);
}

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _mongoCollection;

    public ProductRepository(IMongoDbContext dbContext)
    {
        _mongoCollection = dbContext.GetCollection<Product>(typeof(Product).Name);
    }
    public async Task<PagedResult<Product>> GetAll(ProductFilter filter)
    {
        var builder = Builders<Product>.Filter;
        var filterDefinition = builder.Empty;

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            filterDefinition &= builder.Regex("Name", new BsonRegularExpression(filter.Name, "i"));
        }

        if (filter.MinPrice.HasValue)
        {
            filterDefinition &= builder.Gte("Price", filter.MinPrice.Value);
        }

        if (filter.OnlyActive.HasValue)
        {
            filterDefinition &= builder.Eq("Active", filter.OnlyActive.Value);
        }

        var results = await _mongoCollection
            .Find(filterDefinition)
            .Sort(filter.GetSortDefinition<Product>())
            .Skip((filter.Page - 1) * filter.PageSize)
            .Limit(filter.PageSize)
            .ToListAsync();

        var totalItems = await _mongoCollection.CountDocumentsAsync(filterDefinition);

        return new PagedResult<Product>
        {
            Items = results,
            TotalCount = (int)totalItems,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
}
