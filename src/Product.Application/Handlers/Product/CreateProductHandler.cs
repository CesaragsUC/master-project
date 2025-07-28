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
public class CreateProductHandler : 
    CommandValidator,
    IRequestHandler<CreateProductCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly IBobStorageService _bobStorageService;
    private readonly IProductService _productService;

    static readonly ActivitySource activitySource = new("Product.Api");

    public CreateProductHandler(
        IBobStorageService bobStorageService,
        IProductService productService,
        IProductRepository productRepository)
    {
        _bobStorageService = bobStorageService;
        _productService = productService;
        _productRepository = productRepository;

    }

    public async Task<Result<bool>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        using var activity = activitySource.StartActivity("CreateProduct");

        try
        {

            var validationResponse = await Validator(request, new CreateProductCommandValidator());

            if (!validationResponse.Success)
                return  await Result<bool>.FailureAsync(400, validationResponse.Errors?.ToList()!);

            var product = await ToProduct(request);
            await _productRepository.AddAsync(product);
            await _productRepository.Commit();

            if (activity != null)
            {
                activity.SetTag("productName", request.Name!);
                activity.SetTag("price", request.Price);

                activity.AddEvent(new ActivityEvent("ProductCreated"));
            }


            await SendMessage(product);

        }
        catch(DbUpdateException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            Log.Error(ex, "Error while saving product to database: {Message}", ex.Message);
            return await Result<bool>.FailureAsync(500, ex.Message!);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            Log.Error(ex, "Error while save product to database");
            return await Result<bool>.FailureAsync(500, ex.Message!); 
        }


        return await Result<bool>.SuccessAsync(); 
    }

    private async Task<Domain.Models.Product> ToProduct(CreateProductCommand request)
    {
        return  new Domain.Models.Product
        {
            Name = request.Name,
            Price = request.Price,
            Active = request.Active,
            CreatAt = DateTime.Now,
            ImageUri = await UploadImage(request.ImageBase64)

        };
    }

    private async Task<string> UploadImage(string? base64Image)
    {
        if (!string.IsNullOrEmpty(base64Image))
            return await _bobStorageService.UploadBase64ImageToBlobAsync(base64Image, Guid.NewGuid().ToString());
        else
            return string.Empty;
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
