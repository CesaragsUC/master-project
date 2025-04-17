using MediatR;
using Product.Application.Comands.Product;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using Product.Domain.Exceptions;
using ResultNet;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Handlers.Product;

[ExcludeFromCodeCoverage]
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductService _productService;

    public DeleteProductHandler(
        IProductService productService,
       IProductRepository productRepository)
    {
        _productService = productService;
        _productRepository = productRepository;
    }
    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty) return await Result<bool>.FailureAsync(400,"Invalid Id");

            var produto = _productRepository.FindOne(request.Id);
            if (produto == null) return await Result<bool>.FailureAsync(400, $"Product {request.Id} not find");

            _productRepository.Delete(produto);
            await _productRepository.Commit();

            await _productService.PublishProductDeletedEvent(new ProductDeletedDomainEvent
            {
                ProductId = produto.Id.ToString()
            });

            return  await Result<bool>.SuccessAsync($"Product {request.Id} deleted successfuly");
        }
        catch (ProductInvalidException ex)
        {
            Log.Error(ex, "Error on delete product {Id}", request.Id);
            throw new ProductInvalidException("erro interno");
        }


    }
}
