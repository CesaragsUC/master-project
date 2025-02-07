

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Order.Domain.Entities;

[ExcludeFromCodeCoverage]
public class OrderItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }

    [JsonIgnore]
    public Order? Order { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

}
