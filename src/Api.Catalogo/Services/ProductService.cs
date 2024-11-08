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

public class ProductService : IProductService
{
    private readonly IMongoRepository<Product> _repository;
    private readonly IProductRepository _produtoRepository;

    public ProductService(IMongoRepository<Product> repository, IProductRepository produtoRepository)
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

    public async Task<ResponseResult<List<ProductDto>>> GetAll(ProductFilter filtro)
    {
        try
        {
            if(filtro is null)
            {
                return new ResponseResult<List<ProductDto>>
                {
                    Data = null,
                    Success = false,
                    Errors = new string[] { "Filtro inválido" }
                };
            }

            var produtos = await _produtoRepository.GetAll(filtro);

            return new ResponseResult<List<ProductDto>>
            {
                Data = produtos.Items?.Select(p => (ProductDto)p).ToList(),
                Success = true,
                TotalItems = produtos.Items!.Count,
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                TotalPages = (int)Math.Ceiling((double)produtos.Items!.Count / filtro.PageSize)
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<List<ProductDto>>
            {
                Data = null,
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }

    }


    public async Task<ResponseResult<ProductDto>> GetById(string field, Guid id)
    {
        try
        {
            var produto = await _repository.GetById(field, id);
            if (produto == null)
            {
                return new ResponseResult<ProductDto>
                {
                    Success = false,
                    Errors = new string[] { "Produto não encontrado" }
                };
            }

            return new ResponseResult<ProductDto>
            {
                Data = new ProductDto
                {
                    ProductId = produto.ProductId,
                    Name = produto.Name,
                    Price = produto.Price,
                    Active = produto.Active,
                    CreatAt = produto.CreatAt
                },
                Success = true

            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<ProductDto>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }
    }

    public async Task<ResponseResult<List<ProductDto>>> GetByName(string field, string nome)
    {
        try
        {
            var produtos = await _repository.GetByName(field, nome);

            if (produtos.Items is null)
            {
                return new ResponseResult<List<ProductDto>>
                {
                    Success = false,
                    Errors = new string[] { "Produto não encontrado" }
                };
            }

            return new ResponseResult<List<ProductDto>>
            {
                Data = produtos.Items?.Select(p => (ProductDto)p).ToList(),
                Success = true,
                TotalItems = produtos.Items!.Count!,
            };
        }
        catch (Exception ex)
        {
            return new ResponseResult<List<ProductDto>>
            {
                Success = false,
                Errors = new string[] { ex.Message }
            };
        }

    }

    public async Task<ResponseResult<bool>> Insert(ProductCreateDto obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new ProductCreateValidator());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            await _repository.Insert((Product)obj);

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

    public async Task<ResponseResult<bool>> InsertMany(List<ProductCreateDto> obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new AddProductListDto());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            var produtos = obj.Select(p => new Product
            {
                Name = p.Name,
                Price = Convert.ToDecimal(p.Price),
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

    public async Task<ResponseResult<bool>> Update(ProductUpdateDto obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new ProductUpdateValidator());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            await _repository.UpdateAsync(nameof(obj.ProductId), (Product)obj);

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
    public async Task<ResponseResult<bool>> DemoUpdate2(ProductUpdateDto obj)
    {
        try
        {
            var validationResponse = await ProdutoValidation(obj, new ProductUpdateValidator());

            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            // Define a condição para encontrar o produto com o Id especificado
            Expression<Func<Product, bool>> whereCondition = p => p.ProductId == obj.ProductId;

            // Define o campo que deseja atualizar 
            Expression<Func<Product, decimal?>> fieldToUpdate = p => p.Price;

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