using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Orders;

[ExcludeFromCodeCoverage]
public class OrderUpdateddEvent : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public int Status { get; set; }
}