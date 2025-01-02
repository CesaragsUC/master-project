using System.Diagnostics.CodeAnalysis;

namespace Basket.Domain.Entities;


[ExcludeFromCodeCoverage]
public class Cart
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
}
