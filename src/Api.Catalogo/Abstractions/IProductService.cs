using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Api.Catalogo.Models;

namespace Api.Catalogo.Abstractions;

public interface IProductService
{
    Task<ResponseResult<List<ProductDto>>> GetAll(ProductFilter filtro);
    Task<ResponseResult<ProductDto>> GetById(string field, Guid id);
    Task<ResponseResult<List<ProductDto>>> GetByName(string field, string nome);
    Task<ResponseResult<bool>> Insert(ProductCreateDto obj);
    Task<ResponseResult<bool>> InsertMany(List<ProductCreateDto> obj);
    Task<ResponseResult<bool>> Update(ProductUpdateDto obj);
    Task<ResponseResult<bool>> Delete(string field, Guid id);
    Task<ResponseResult<bool>> DeleteByName(string field, string nome);

}
