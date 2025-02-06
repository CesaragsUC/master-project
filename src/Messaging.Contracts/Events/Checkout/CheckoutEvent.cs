using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Checkout;

[ExcludeFromCodeCoverage]
public class CheckoutEvent : IRequest<bool>
{
    public Guid CustomerId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public List<CartItensDto> Items { get; set; } = new();

    public decimal SubTotal => Items.Sum(item => item.TotalPrice);

    public decimal TotalPrice { get; set; }

    public string? PaymentToken { get; set; }

    public int? Status { get; set; }
}
