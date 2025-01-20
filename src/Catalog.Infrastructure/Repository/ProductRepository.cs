using Catalog.Domain.Abstractions;
using Catalog.Domain.Models;
using Catalog.Service.Abstractions;
using Catalog.Services.Filters;
using EasyMongoNet.Abstractions;
using EasyMongoNet.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Catalog.Infrastructure.Repository;


[ExcludeFromCodeCoverage]
public class ProductRepository : IProductRepository
{
    private readonly IMongoRepository<Products> _repository;

    public ProductRepository(IMongoRepository<Products> repository)
    {
        _repository = repository;

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

        var results = await _repository.GetAllAsync(filterDefinition,filter.Page, filter.PageSize,filter.OrderDirection!);

        return results;
    }
}