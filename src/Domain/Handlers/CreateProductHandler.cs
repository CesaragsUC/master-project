using Application.Dtos.Dtos.Response;
using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using Domain.Validation;
using FluentValidation;
using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Product;
using Serilog;

namespace Domain.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<bool>>
    {
        private readonly IRepository<Product> _repository;
        private readonly IPublishEndpoint _publish;
        private readonly IBobStorageService _bobStorageService;


        public CreateProductHandler(IRepository<Product> repository,
            IPublishEndpoint publish,
            IBobStorageService bobStorageService)
        {
            _repository = repository;
            _publish = publish;
            _bobStorageService = bobStorageService;
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

                var produto = new Product
                {
                    Name = request.Name,
                    Price = request.Price,
                    Active = request.Active,
                    CreatAt = DateTime.Now,
                    ImageUri =  await UploadImage(request.ImageBase64)

                };

                await _repository.Add(produto);

                await _publish.Publish<ProductAddedEvent>(new ProductAddedEvent
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
                Log.Error(ex, "Erro ao adicionar produto");
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
}
