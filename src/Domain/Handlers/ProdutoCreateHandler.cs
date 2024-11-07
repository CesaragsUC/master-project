using Domain.Events;
using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using Domain.Validation;
using FluentValidation;
using MassTransit;
using MediatR;
using Serilog;

namespace Domain.Handlers
{
    public class ProdutoCreateHandler : IRequestHandler<CreateProdutoCommand, bool>
    {
        private readonly IRepository<Produtos> _repository;
        private readonly IPublishEndpoint _publish;
        private readonly IBobStorageService _bobStorageService;


        public ProdutoCreateHandler(IRepository<Produtos> repository,
            IPublishEndpoint publish,
            IBobStorageService bobStorageService)
        {
            _repository = repository;
            _publish = publish;
            _bobStorageService = bobStorageService;
        }

        public async Task<bool> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResponse = await ProdutoValidation(request, new CriarProdutoCommandValidation());

                if (!validationResponse.Success)
                {
                    return false;
                }

                var produto = new Produtos
                {
                    Nome = request.Nome,
                    Preco = request.Preco,
                    Active = request.Active,
                    CreatAt = DateTime.Now,
                    ImageUri =  await UploadImage(request.ImageBase64)

                };

                await _repository.Add(produto);

                await _publish.Publish<ProdutoAdicionadoEvent>(new ProdutoAdicionadoEvent
                {
                    ProdutoId = produto.Id.ToString(),
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    Active = produto.Active,
                    CreatAt = produto.CreatAt,
                    ImageUri = produto.ImageUri
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao adicionar produto");
                return false;
            }


            return true;
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
