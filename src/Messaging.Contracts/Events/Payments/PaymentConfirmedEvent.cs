using MediatR;

namespace Messaging.Contracts.Events.Payments;

public class PaymentConfirmedEvent : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; }
    public int Status { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? PaymentToken { get; set; }
}
