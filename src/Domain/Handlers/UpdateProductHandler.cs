﻿
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
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
    {
        private readonly IPublishEndpoint _publish;
        private readonly IRepository<Product> _repository;
        private readonly IBobStorageService _bobStorageService;

        public UpdateProductHandler
            (IRepository<Product> repository,
            IBobStorageService bobStorageService,
            IPublishEndpoint publish)
        {
            _repository = repository;
            _bobStorageService = bobStorageService;
            _publish = publish;
        }

        public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResponse = await ProdutoValidacao(request, new UpdateProductCommandValidator());

                if (!validationResponse.Success)
                {
                    return  await Result<bool>.FailureAsync(400, validationResponse?.Errors?.ToList()); 
                }

                var produto = _repository.FindOne(x => x.Id == request.Id);

                if (produto == null) return await Result<bool>.FailureAsync(400, $"Product {request.Id} not find"); 

                produto.Name = request.Name;
                produto.Price = request.Price;
                produto.Active = request.Active;

                if(!string.IsNullOrEmpty(request.ImageBase64))
                    produto.ImageUri = await UploadImage(request.ImageBase64, produto.ImageUri);

                await _repository.Update(produto);

                await _publish.Publish<ProductUpdatedEvent>(new ProductUpdatedEvent
                {
                    ProductId = produto.Id.ToString(),
                    Name = produto.Name,
                    Price = produto.Price,
                    Active = produto.Active,
                    ImageUri = produto.ImageUri
                });

                return await Result<bool>.SuccessAsync($"Product {request.Id} updated successfuly");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fail to updated product");
                return  await Result<bool>.FailureAsync(400, ex.Message); 
            }
        }
        private async Task<string> UploadImage(string? base64Image,string? oldBlobName)
        {
            if (!string.IsNullOrEmpty(base64Image))
                return await _bobStorageService.UploadBase64ImageToBlobAsync(base64Image, Guid.NewGuid().ToString(), oldBlobName);
            else
                return string.Empty;
        }

        private async Task<ResponseResult<bool>> ProdutoValidacao<T>(T obj, AbstractValidator<T> validator)
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