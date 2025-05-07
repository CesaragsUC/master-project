using Catalog.Application.Dtos;
using Catalog.Domain.Filters;
using Catalog.Domain.Models;

namespace Catalog.Application.Abstractions;

public interface IProductService
{
    Task<ResponseResult<List<ProductDto>>> GetAll(ProductFilter filtro);
    Task<ResponseResult<ProductDto>> GetById(string field, Guid id);
    Task<ResponseResult<List<ProductDto>>> GetByName(string name);
    Task<ResponseResult<bool>> Insert(ProductCreateDto obj);
    Task<ResponseResult<bool>> InsertMany(List<ProductCreateDto> obj);
    Task<ResponseResult<bool>> Update(ProductUpdateDto obj);
    Task<ResponseResult<bool>> Delete(string field, Guid id);

}
