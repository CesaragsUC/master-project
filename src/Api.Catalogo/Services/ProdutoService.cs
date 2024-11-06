using Api.Catalogo.Abstractions;
using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Api.Catalogo.Models;
using Api.Catalogo.Repository;
using Api.Catalogo.Validation;
using FluentValidation;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Api.Catalogo.Services;

public class ProdutoService : IProdutoService
{
    private readonly IMongoRepository<Produtos> _repository;
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IMongoRepository<Produtos> repository, IProdutoRepository produtoRepository)
    {
        _repository = repository;
        _produtoRepository = produtoRepository;
    }

    public async Task<ResponseResult<bool>> Delete(string field, Guid id)
    {
        try
        {
            if (id == Guid.Empty ||  string.IsNullOrEmpty(field))
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Errors = new string[] { "Id inválido" }
                };
            }

            await _repository.Delete(field, id);

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Produto excluido com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }

    }

    public async Task<ResponseResult<bool>> DeleteByName(string field, string nome)
    {
        try
        {
            await _repository.DeleteByName(field, nome);

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Produto excluido com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }
    }

    public async Task<ResponseResult<List<ProdutoDto>>> GetAll(ProdutoFilter filtro)
    {
        try
        {
            if(filtro is null)
            {
                return new ResponseResult<List<ProdutoDto>>
                {
                    Data = null,
                    Success = false,
                    Errors = new string[] { "Filtro inválido" }
                };
            }

            var produtos = await _produtoRepository.GetAll(filtro);

            return new ResponseResult<List<ProdutoDto>>
            {
                Data = produtos.Items?.Select(p => (ProdutoDto)p).ToList(),
                Success = true,
                TotalItems = produtos.Items!.Count,
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                TotalPages = (int)Math.Ceiling((double)produtos.Items!.Count / filtro.PageSize)
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<List<ProdutoDto>>
            {
                Data = null,
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }

    }


    public async Task<ResponseResult<ProdutoDto>> GetById(string field, Guid id)
    {
        try
        {
            var produto = await _repository.GetById(field, id);
            if (produto == null)
            {
                return new ResponseResult<ProdutoDto>
                {
                    Success = false,
                    Errors = new string[] { "Produto não encontrado" }
                };
            }

            return new ResponseResult<ProdutoDto>
            {
                Data = new ProdutoDto
                {
                    ProdutoId = produto.ProdutoId,
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    Active = produto.Active,
                    CreatAt = produto.CreatAt
                },
                Success = true

            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<ProdutoDto>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }
    }

    public async Task<ResponseResult<List<ProdutoDto>>> GetByName(string field, string nome)
    {
        try
        {
            var produtos = await _repository.GetByName(field, nome);

            if (produtos.Items is null)
            {
                return new ResponseResult<List<ProdutoDto>>
                {
                    Success = false,
                    Errors = new string[] { "Produto não encontrado" }
                };
            }

            return new ResponseResult<List<ProdutoDto>>
            {
                Data = produtos.Items?.Select(p => (ProdutoDto)p).ToList(),
                Success = true,
                TotalItems = produtos.Items!.Count!,
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<List<ProdutoDto>>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }

    }

    public async Task<ResponseResult<bool>> Insert(ProdutoAddDto obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new ProdutoAddDtoValidation());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            await _repository.Insert((Produtos)obj);

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Produto inserido com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }
    }

    public async Task<ResponseResult<bool>> InsertMany(List<ProdutoAddDto> obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new ProdutoAddDtoListValidation());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            var produtos = obj.Select(p => new Produtos
            {
                Nome = p.Nome,
                Preco = Convert.ToDecimal(p.Preco),
                Active = Convert.ToBoolean(p.Active),
                CreatAt = DateTime.Now
            }).ToList();

            await _repository.InsertMany(produtos);

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Produtos inserido com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }
    }

    public async Task<ResponseResult<bool>> Update(ProdutoUpdateDto obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new ProdutoUpdateDtoValidation());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            await _repository.UpdateAsync(nameof(obj.ProdutoId), (Produtos)obj);

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Produtos atualizado com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };

        }
    }

    // Exemplo como usar o UpdateAsync com Expression
    public async Task<ResponseResult<bool>> DemoUpdate2(ProdutoUpdateDto obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new ProdutoUpdateDtoValidation());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            // Define a condição para encontrar o produto com o Id especificado
            Expression<Func<Produtos, bool>> whereCondition = p => p.ProdutoId == obj.ProdutoId;

            // Define o campo que deseja atualizar 
            Expression<Func<Produtos, decimal?>> fieldToUpdate = p => p.Preco;

            // Chama o método UpdateAsync para atualizar o preço do produto
            await _repository.UpdateAsync(whereCondition, fieldToUpdate, obj.Preco);

            return new ResponseResult<bool>
            {
                Success = true,
                Message = "Produtos atualizado com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };

        }
    }

    private async Task<ResponseResult<bool>> ProdutoValidation<T>(T obj, AbstractValidator<T> validator)
    {
        var validationResult = validator.Validate(obj);
        var result = new ResponseResult<bool>();

        if (!validationResult.IsValid)
        {
            result.Success = false;
            result.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return result;
        }

        result.Success = true;
        return result;
    }

}