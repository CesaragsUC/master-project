using Discount.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Domain.Dtos;

[ExcludeFromCodeCoverage]
public record CouponCreateDto
{
    public string? Code { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public decimal MinValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool Active { get; set; }
    public int? MaxUse { get; set; }
}
