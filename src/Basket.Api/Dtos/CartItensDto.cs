namespace Basket.Api.Dtos;

public record CartItensDto
{
    public string? ProductId { get; init; }
    public string? ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => Quantity * UnitPrice;
}