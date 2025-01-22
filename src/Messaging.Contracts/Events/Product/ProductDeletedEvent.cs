using MediatR;

namespace Messaging.Contracts.Events.Product;

public class ProductDeletedEvent : IRequest<bool>
{
    public string? ProductId { get; set; }
}