using HybridRepoNet.Abstractions;
using Infrastructure;
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
    private readonly IUnitOfWork<ProductDbContext> _unitOfWork;
    private readonly IProductService _productService;

    public DeleteProductHandler(
        IProductService productService,
        IUnitOfWork<ProductDbContext> unitOfWork)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty) return await Result<bool>.FailureAsync(400,"Invalid Id");

            var produto = _unitOfWork.Repository<Domain.Models.Product>().FindOne(x => x.Id == request.Id);
            if (produto == null) return await Result<bool>.FailureAsync(400, $"Product {request.Id} not find"); 

            _unitOfWork.Repository<Domain.Models.Product>().Delete(produto);
            await _unitOfWork.Commit();

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
