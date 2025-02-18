

using HybridRepoNet;
using System.Diagnostics.CodeAnalysis;

namespace Order.Domain.Entities;

[ExcludeFromCodeCoverage]
public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public IEnumerable<OrderItem>? Items { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public string? Name { get; set; }
    public string? PaymentToken { get; set; }
}
