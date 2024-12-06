using Application.Dtos.Dtos.Response;
using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Product;
using Serilog;

namespace Domain.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IRepository<Product> _repository;
        private readonly IPublishEndpoint _publish;

        public DeleteProductHandler(IRepository<Product> repository, IPublishEndpoint publish)
        {
            _repository = repository;
            _publish = publish;
        }
        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == Guid.Empty) return await Result<bool>.FailureAsync(500,"Invalid Id");

                var produto = _repository.FindOne(x => x.Id == request.Id);
                if (produto == null) return await Result<bool>.FailureAsync(400, $"Product {request.Id} not find"); 

                await _repository.Delete(produto);

                await _publish.Publish<ProductDeletedEvent>(new ProductDeletedEvent
                {
                    ProductId = produto.Id.ToString(),
                });

                return  await Result<bool>.SuccessAsync($"Product {request.Id} deleted successfuly");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error on delete product {Id}", request.Id);
                throw;
            }


        }
    }
}
