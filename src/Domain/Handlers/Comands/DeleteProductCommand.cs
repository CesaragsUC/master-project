using MediatR;

namespace Domain.Handlers.Comands
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
