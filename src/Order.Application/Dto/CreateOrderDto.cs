using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Dto;

[ExcludeFromCodeCoverage]
public sealed record class CreateOrderDto
{
    public DateTime CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public IEnumerable<OrderItemDto>? Items { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public string? Name { get; set; }
    public string? PaymentToken { get; set; }
}
