using System.Diagnostics.CodeAnalysis;

namespace Discount.Domain.Dtos;

[ExcludeFromCodeCoverage]
public record CouponDeleteDto
{
    public Guid Id { get; set; }

}