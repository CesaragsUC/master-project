using AutoMapper;
using Catalog.Application.Abstractions;
using Catalog.Application.Dtos;
using Catalog.Application.Validation;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Filters;
using Catalog.Domain.Models;
using EasyMongoNet.Abstractions;
using FluentValidation;
using MongoDB.Driver;

namespace Catalog.Application.Services;

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

        var validationResponse = await ProdutoValidation(obj, new ProductCreateValidator());

        if (!validationResponse.Success)
        {
            return validationResponse;
        }

        var product = _mapper.Map<Products>(obj);

        await _repository.InsertOneAsync(product);

        return new ResponseResult<bool>
        {
            Success = true,
            Message = "Produto inserido com sucesso"
        };

    }

    public async Task<ResponseResult<bool>> InsertMany(List<ProductCreateDto> obj)
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
            Active = Convert.ToBoolean(p.Active)
        }).ToList();

        await _repository.InsertManyAsync(produtos);

        return new ResponseResult<bool>
        {
            Success = true,
            Message = "Produtos inserido com sucesso"
        };

    }

    public async Task<ResponseResult<bool>> Update(ProductUpdateDto obj)
    {

        var validationResponse = await ProdutoValidation(obj, new ProductUpdateValidator());

        if (!validationResponse.Success)
        {
            return validationResponse;
        }

        var product = await _repository.FindByIdAsync(x => x.ProductId == obj.ProductId);

        if (product == null)
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { "Produto não encontrado" }
            };
        }

        product.ModifiedAt = DateTime.Now;
        product.Name = obj.Name;
        product.Price = obj.Price;
        product.Active = obj.Active;
        product.ImageUri = obj.ImageUri;

        await _repository.UpdateAsync(product);

        return new ResponseResult<bool>
        {
            Success = true,
            Message = "Produtos atualizado com sucesso"
        };
    }


    public async Task<ResponseResult<bool>> Delete(string field, Guid id)
    {

        if (id == Guid.Empty || string.IsNullOrEmpty(field))
        {
            return new ResponseResult<bool>
            {
                Success = false,
                Errors = new string[] { "Id inválido" }
            };
        }

        await _repository.DeleteOneAsync(x => x.ProductId == id.ToString());

        return new ResponseResult<bool>
        {
            Success = true,
            Message = "Produto excluido com sucesso"
        };


    }

    public async Task<ResponseResult<List<ProductDto>>> GetAll(ProductFilter filtro)
    {

        if (filtro is null)
        {
            return new ResponseResult<List<ProductDto>>
            {
                Data = null,
                Success = false,
                Errors = new string[] { "Invalid filter" }
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


    public async Task<ResponseResult<ProductDto>> GetById(string field, Guid id)
    {
        var produto = await _repository.FindOneAsync(x => x.ProductId == id.ToString());
        if (produto == null)
        {
            return new ResponseResult<ProductDto>
            {
                Success = false,
                Errors = new string[] { "Product not found" }
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
                CreatAt = produto.CreatedAt,
                ImageUri = produto.ImageUri
            },
            Success = true

        };

    }

    public async Task<ResponseResult<List<ProductDto>>> GetByName(string name)
    {
        var produtos = await _repository.FilterBy(x => x.Name.Contains(name));

        if (!produtos.Any())
        {
            return new ResponseResult<List<ProductDto>>
            {
                Success = false,
                Errors = new string[] { "Product not found" }
            };
        }

        return new ResponseResult<List<ProductDto>>
        {
            Data = produtos.Select(p => _mapper.Map<ProductDto>(p)).ToList(),
            Success = true,
            TotalItems = produtos.ToList().Count!,
        };


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