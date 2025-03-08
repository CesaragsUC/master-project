using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountRequest
{
    public Guid CustomerId { get; set; }
    public string? CouponCode { get; set; }
    public decimal TotalPrice { get; set; }

}