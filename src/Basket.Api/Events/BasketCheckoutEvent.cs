using Basket.Api.Dtos;

namespace Basket.Api.Events;

public class BasketCheckoutEvent
{
    public string? CustomerId { get; set; }

    public string? UserName { get; set; }
    public string? Email { get; set; }

    public List<CartItensDto> Items { get; set; } = new();

    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
}
