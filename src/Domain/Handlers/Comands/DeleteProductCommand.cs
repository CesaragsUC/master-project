using Application.Dtos.Dtos.Response;
using MediatR;

namespace Domain.Handlers.Comands
{
    public class DeleteProductCommand : IRequest<Result<bool>>
    {
        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }
    }
}
