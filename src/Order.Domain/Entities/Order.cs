

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Order.Domain.Entities;

[ExcludeFromCodeCoverage]
public class Order
{
    [Key]
    public Guid Id { get; set; } =  Guid.NewGuid(); 
    public DateTime CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderItem>? Items { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public string? Name { get; set; }
    public string? PaymentToken { get; set; }
}
