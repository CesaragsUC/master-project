using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountResponse
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public int Type { get; set; }
    public decimal Value { get; set; }
    public decimal MinValue { get; set; }
}
