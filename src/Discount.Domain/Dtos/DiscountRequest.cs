using System.Diagnostics.CodeAnalysis;

namespace Discount.Domain.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountRequest
{
    public string? Code { get; set; }
    public decimal Total { get; set; }
}
