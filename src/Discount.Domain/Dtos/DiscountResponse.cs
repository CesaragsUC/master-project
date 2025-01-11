using System.Diagnostics.CodeAnalysis;

namespace Discount.Domain.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountResponse
{
    public decimal TotalDiscount { get; set; }
}
