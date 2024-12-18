using Catalog.Domain.Abstractions;
using Catalog.Domain.Models;
using Catalog.Service.Abstractions;
using Catalog.Services.Filters;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Catalog.Infrastructure.Repository;


[ExcludeFromCodeCoverage]
public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Products> _mongoCollection;

    public ProductRepository(IMongoDbContext dbContext)
    {
        _mongoCollection = dbContext.GetCollection<Products>(typeof(Products).Name);
    }
    public async Task<PagedResult<Products>> GetAll(ProductFilter filter)
    
    {
        var builder = Builders<Products>.Filter;
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
            .Sort(filter.GetSortDefinition<Products>())
            .Skip((filter.Page - 1) * filter.PageSize)
            .Limit(filter.PageSize)
            .ToListAsync();

        var totalItems = await _mongoCollection.CountDocumentsAsync(filterDefinition);

        return new PagedResult<Products>
        {
            Items = results,
            TotalCount = (int)totalItems,
            Page = filter.Page,
            PageSize = filter.PageSize

        };
    }
}