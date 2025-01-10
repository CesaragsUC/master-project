namespace Basket.Api.Dtos;

public class DiscountRequest
{
    public string? Code { get; set; }
    public decimal TotalPrice { get; set; }
}
