using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Checkout;

[ExcludeFromCodeCoverage]
public record CartItensDto
{
    public Guid ProductId { get; init; }
    public string? ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string? ImageUrl { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}