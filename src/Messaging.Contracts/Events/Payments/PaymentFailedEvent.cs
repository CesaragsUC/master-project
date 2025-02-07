using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Payments;

[ExcludeFromCodeCoverage]
public class PaymentFailedEvent : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; }
    public int Status { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? PaymentToken { get; set; }
}
