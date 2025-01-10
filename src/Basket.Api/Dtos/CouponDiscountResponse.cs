namespace Basket.Api.Dtos;

public class DiscountResponse
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public int Type { get; set; }
    public decimal Value { get; set; }
    public decimal MinValue { get; set; }
}
