using System.Diagnostics.CodeAnalysis;

namespace Discount.Api.Dtos;


[ExcludeFromCodeCoverage]
public record CouponDiscountDeleteDto
{
    public Guid Id { get; set; }

}