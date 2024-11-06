using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Api.Catalogo.Models;

namespace Api.Catalogo.Abstractions;

public interface IProdutoService
{
    Task<ResponseResult<List<ProdutoDto>>> GetAll(ProdutoFilter filtro);
    Task<ResponseResult<ProdutoDto>> GetById(string field, Guid id);
    Task<ResponseResult<List<ProdutoDto>>> GetByName(string field, string nome);
    Task<ResponseResult<bool>> Insert(ProdutoAddDto obj);
    Task<ResponseResult<bool>> InsertMany(List<ProdutoAddDto> obj);
    Task<ResponseResult<bool>> Update(ProdutoUpdateDto obj);
    Task<ResponseResult<bool>> Delete(string field, Guid id);
    Task<ResponseResult<bool>> DeleteByName(string field, string nome);

}
