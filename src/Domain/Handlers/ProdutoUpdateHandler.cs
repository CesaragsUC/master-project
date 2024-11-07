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
    public class ProdutoUpdateHandler : IRequestHandler<UpdateProdutoCommand, bool>
    {
        private readonly IPublishEndpoint _publish;
        private readonly IRepository<Produtos> _repository;
        private readonly IBobStorageService _bobStorageService;

        public ProdutoUpdateHandler(IRepository<Produtos> repository,
            IBobStorageService bobStorageService,
            IPublishEndpoint publish)
        {
            _repository = repository;
            _bobStorageService = bobStorageService;
            _publish = publish;
        }

        public async Task<bool> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResponse = await ProdutoValidation(request, new AtualizarProdutoCommandValidation());

                if (!validationResponse.Success)
                {
                    return false;
                }

                var produto = _repository.FindOne(x => x.Id == request.Id);

                if (produto == null) return false;

                produto.Nome = request.Nome;
                produto.Preco = request.Preco;
                produto.Active = request.Active;

                if(!string.IsNullOrEmpty(request.ImageBase64))
                    produto.ImageUri = await UploadImage(request.ImageBase64, produto.ImageUri);

                await _repository.Update(produto);

                await _publish.Publish<ProdutoAtualizadoEvent>(new ProdutoAtualizadoEvent
                {
                    ProdutoId = produto.Id.ToString(),
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    Active = produto.Active,
                    ImageUri = produto.ImageUri
                });

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar produto");
                return false;
            }
        }
        private async Task<string> UploadImage(string? base64Image,string? oldBlobName)
        {
            if (!string.IsNullOrEmpty(base64Image))
                return await _bobStorageService.UploadBase64ImageToBlobAsync(base64Image, Guid.NewGuid().ToString(), oldBlobName);
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
