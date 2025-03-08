using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Orders;

[ExcludeFromCodeCoverage]
public class DeleteCartEvent : IRequest<bool>
{
    public Guid CustomerId { get; set; }
}