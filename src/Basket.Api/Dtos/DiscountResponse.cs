using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountResultResponse
{
    public decimal TotalPrice { get; set; }
    public decimal? DiscountApplied { get; set; }
    public decimal? SubTotal { get; set; }

}
