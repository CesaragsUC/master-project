using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Dtos;

[ExcludeFromCodeCoverage]
public sealed record class OrderDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public IEnumerable<OrderItemDto>? Items { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public string? Name { get; set; }
    public string? PaymentToken { get; set; }
}