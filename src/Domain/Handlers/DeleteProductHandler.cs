using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Domain.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IRepository<Product> _repository;

        public DeleteProductHandler(IRepository<Product> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) return false;

            var produto =  _repository.FindOne(x=> x.Id == request.Id);
            if (produto == null) return false;

            await _repository.Delete(produto);

            return true;
        }
    }
}
