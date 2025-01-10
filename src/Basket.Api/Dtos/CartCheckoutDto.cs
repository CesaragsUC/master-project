using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class CartCheckoutDto
{
    public string? CustomerId { get; set; }

    public string? UserName { get; set; }
    public string? Email { get; set; }

    public List<CartItensDto> Items { get; set; } = new();

    public decimal TotalPrice { get; set; }
}