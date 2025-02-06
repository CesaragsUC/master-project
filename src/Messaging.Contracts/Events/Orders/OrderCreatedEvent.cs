using Amazon.Runtime.Internal;
using MediatR;

namespace Messaging.Contracts.Events.Orders;

public class OrderCreatedEvent : IRequest<bool>
{
    public Guid CustomerId { get; set; }
    public int? Status { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public List<OrderItemDto>? Items { get; set; }
    public decimal Total { get; set; }
    public string? PaymentToken { get; set; }
}
