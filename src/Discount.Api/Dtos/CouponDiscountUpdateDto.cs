using Discount.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Api.Dtos;


[ExcludeFromCodeCoverage]
public record CouponDiscountUpdateDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public decimal MinValue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDae { get; set; }
    public bool Active { get; set; }
    public int? MaxUse { get; set; }
    public int TotalUse { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
