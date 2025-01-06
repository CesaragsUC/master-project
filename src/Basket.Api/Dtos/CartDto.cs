namespace Basket.Api.Dtos;


public class CartDto
{
    public string? CustomerId { get; set; }

    public List<CartItensDto> Items { get; set; } = new();

    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
}