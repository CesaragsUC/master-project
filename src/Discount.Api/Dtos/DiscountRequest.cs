using System.Diagnostics.CodeAnalysis;

namespace Discount.Api.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountRequest
{
    public string? Code { get; set; }
    public decimal TotalPrice { get; set; }
}
