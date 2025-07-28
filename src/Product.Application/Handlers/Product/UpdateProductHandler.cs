using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Application.Comands.Product;
using Product.Application.Validation;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using ResultNet;
using Serilog;
using Shared.Kernel.Validation;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Handlers.Product;

[ExcludeFromCodeCoverage]
public class UpdateProductHandler :
    CommandValidator,
    IRequestHandler<UpdateProductCommand, Result<bool>>
{
    private readonly IProductService _productService;
    private readonly IProductRepository _productRepository;
    private readonly IBobStorageService _bobStorageService;
    static readonly ActivitySource activitySource = new("Product.Api");
    public UpdateProductHandler(
        IBobStorageService bobStorageService,
        IProductService productService,
       IProductRepository productRepository)
    {
        _bobStorageService = bobStorageService;
        _productService = productService;
        _productRepository = productRepository;
    }

    public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {

        using var activity = activitySource.StartActivity("UpdateProduct");

        try
        {
            var validationResponse = await Validator(request, new UpdateProductCommandValidator());

            if (!validationResponse.Success)
                return await Result<bool>.FailureAsync(400, validationResponse?.Errors?.ToList());

            var existentProduct = _productRepository.FindOne(request.Id);

            if (existentProduct is null )
                return await Result<bool>.FailureAsync(400, $"Product {request.Id} not find");

            var product = await ToProduct(existentProduct!, request);
            _productRepository.Update(product);
            await _productRepository.Commit();

            if (activity != null)
            {
                activity.SetTag("productId", existentProduct.Id!);
                activity.SetTag("productName", existentProduct.Name!);
                activity.SetTag("price", existentProduct.Price);

                activity.AddEvent(new ActivityEvent("ProductUpdated"));
            }


            await SendMessage(product);

            return await Result<bool>.SuccessAsync($"Product {request.Id} updated successfuly");
        }
        catch (DbUpdateException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            Log.Error(ex, "Error while update product. {Message}", ex.Message);
            return await Result<bool>.FailureAsync(500, ex.Message!);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            Log.Error(ex, "Fail to updated product");
            return await Result<bool>.FailureAsync(400, ex.Message);
        }
    }
    private async Task<string> UploadImage(string? base64Image, string? oldBlobName)
    {
        if (!string.IsNullOrEmpty(base64Image))
            return await _bobStorageService.UploadBase64ImageToBlobAsync(base64Image, Guid.NewGuid().ToString(), oldBlobName);
        else
            return string.Empty;
    }

    private async Task<Domain.Models.Product> ToProduct(Domain.Models.Product product,UpdateProductCommand request)
    {
        product.Name = request.Name;
        product.Price = request.Price;
        product.Active = request.Active;

        if (!string.IsNullOrEmpty(request.ImageBase64))
            product.ImageUri = await UploadImage(request.ImageBase64, product.ImageUri);

        return product;
    }

    private async Task SendMessage(Domain.Models.Product product)
    {
        await _productService.PublishProductAddedEvent(new ProductAddedDomainEvent
        {
            ProductId = product.Id.ToString(),
            Name = product.Name,
            Price = product.Price,
            Active = product.Active,
            CreatAt = product.CreatAt,
            ImageUri = product.ImageUri
        });
    }
}
