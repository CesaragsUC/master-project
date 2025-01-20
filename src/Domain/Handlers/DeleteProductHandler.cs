using Domain.Handlers.Comands;
using Domain.Interfaces;
using MediatR;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using Product.Domain.Exceptions;
using ResultNet;
using Serilog;

namespace Product.Domain.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IRepository<Models.Product> _repository;
        private readonly IProductService _productService;

        public DeleteProductHandler(IRepository<Models.Product> repository,
            IProductService productService)
        {
            _repository = repository;
            _productService = productService;
        }
        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == Guid.Empty) return await Result<bool>.FailureAsync(400,"Invalid Id");

                var produto = _repository.FindOne(x => x.Id == request.Id);
                if (produto == null) return await Result<bool>.FailureAsync(400, $"Product {request.Id} not find"); 

                await _repository.Delete(produto);

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
}
