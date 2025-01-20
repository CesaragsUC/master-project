using Catalog.Domain.Models;
using Catalog.Services.Filters;
using EasyMongoNet.Utils;

namespace Catalog.Service.Abstractions;

public interface IProductRepository
{
    Task<PagedResult<Products>> GetAll(ProductFilter filter);
}
