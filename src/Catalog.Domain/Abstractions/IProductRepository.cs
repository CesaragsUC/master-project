using Catalog.Domain.Filters;
using Catalog.Domain.Models;
using EasyMongoNet.Utils;

namespace Catalog.Domain.Abstractions;
public interface IProductRepository
{
    Task<PagedResult<Products>> GetAll(ProductFilter filter);
}
