using Domain.Interfaces;
using FluentValidation;
using HybridRepoNet.Abstractions;
using Infrastructure;
using MediatR;
using Product.Application.Comands.Product;
using Product.Application.Validation;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using Product.Domain.Models;
using ResultNet;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Handlers.Product;

[ExcludeFromCodeCoverage]
public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<bool>>
{
    private readonly  IUnitOfWork<ProductDbContext> _unitOfWork;
    private readonly IBobStorageService _bobStorageService;
    private readonly IProductService _productService;

    public CreateProductHandler(
        IBobStorageService bobStorageService,
        IProductService productService,
        IUnitOfWork<ProductDbContext> unitOfWork)
    {
        _bobStorageService = bobStorageService;
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResponse = await ProdutoValidation(request, new CreateProductCommandValidator());

            if (!validationResponse.Success)
            {
                return  await Result<bool>.FailureAsync(500, validationResponse.Errors?.ToList()!);
            }

            var produto = new Domain.Models.Product
            {
                Name = request.Name,
                Price = request.Price,
                Active = request.Active,
                CreatAt = DateTime.Now,
                ImageUri =  await UploadImage(request.ImageBase64)

            };

            await _unitOfWork.Repository<Domain.Models.Product>().AddAsync(produto);
            await _unitOfWork.Commit();

            await _productService.PublishProductAddedEvent(new ProductAddedDomainEvent
            {
                ProductId = produto.Id.ToString(),
                Name = produto.Name,
                Price = produto.Price,
                Active = produto.Active,
                CreatAt = produto.CreatAt,
                ImageUri = produto.ImageUri
            });


        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while save product to database");
            return await Result<bool>.FailureAsync(500, ex.Message!); 
        }


        return await Result<bool>.SuccessAsync(); 
    }

    private async Task<string> UploadImage(string? base64Image)
    {
        if (!string.IsNullOrEmpty(base64Image))
            return await _bobStorageService.UploadBase64ImageToBlobAsync(base64Image, Guid.NewGuid().ToString());
        else
            return string.Empty;
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
