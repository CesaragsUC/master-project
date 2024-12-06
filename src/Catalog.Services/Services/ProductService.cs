using Application.Dtos.Dtos.Produtos;
using AutoMapper;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Models;
using Catalog.Service.Abstractions;
using Catalog.Service.Validation;
using Catalog.Services.Abstractions;
using Catalog.Services.Filters;
using FluentValidation;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Catalog.Service.Services;

public class ProductService : IProductService
{
    private readonly IMongoRepository<Products> _repository;
    private readonly IProductRepository _produtoRepository;

    private readonly IMapper _mapper;

    public ProductService(IMongoRepository<Products> repository, IProductRepository produtoRepository, IMapper mapper)
    {
        _repository = repository;
        _produtoRepository = produtoRepository;
        _mapper = mapper;
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

            var product = _mapper.Map<Products>(obj);

            await _repository.InsertAsync(product);

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

            var produtos = obj.Select(p => new Products
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

            var product = _mapper.Map<Products>(obj);

            await _repository.UpdateAsync(nameof(obj.ProductId), product);

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
            Expression<Func<Products, bool>> whereCondition = p => p.ProductId == obj.ProductId;

            // Define o campo que deseja atualizar 
            Expression<Func<Products, decimal?>> fieldToUpdate = p => p.Price;

            // Chama o método UpdateAsync para atualizar o preço do produto
            await _repository.UpdateAsync(whereCondition, fieldToUpdate, obj.Price);

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

    public async Task<ResponseResult<bool>> Delete(string field, Guid id)
    {
        try
        {
            if (id == Guid.Empty || string.IsNullOrEmpty(field))
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
            if (filtro is null)
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
                Data = produtos.Items?.Select(p => _mapper.Map<ProductDto>(p)).ToList(),
                Success = true,
                TotalItems = produtos.TotalCount,
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                TotalPages = (int)Math.Ceiling((double)produtos.TotalCount / filtro.PageSize)
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
                    CreatAt = produto.CreatAt,
                    ImageUri = produto.ImageUri
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
                Data = produtos.Items?.Select(p => _mapper.Map<ProductDto>(p)).ToList(),
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