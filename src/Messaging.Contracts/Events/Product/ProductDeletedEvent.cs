using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Product;

[ExcludeFromCodeCoverage]
public class ProductDeletedEvent : IRequest<bool>
{
    public string? ProductId { get; set; }
}