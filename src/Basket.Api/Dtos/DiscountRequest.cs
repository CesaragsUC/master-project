using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountRequest
{
    public string? Code { get; set; }
    public decimal Total { get; set; }
}
