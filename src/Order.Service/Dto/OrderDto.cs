using System.Diagnostics.CodeAnalysis;

namespace Order.Service.Dto;

[ExcludeFromCodeCoverage]
public sealed record class OrderDto
{
    public Guid Id { get; set; } 
    public DateTime CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public IEnumerable<OrderItemDto>? Items { get; set; }
    public decimal TotalAmount => Items!.Sum(item => item.TotalPrice);
    public int Status { get; set; }
}
