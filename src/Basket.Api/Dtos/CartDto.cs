using Messaging.Contracts.Events.Checkout;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class CartDto
{
    public Guid CustomerId { get; set; }

    public List<CartItensDto> Items { get; set; } = new();

    public decimal TotalPrice { get; set; } 

    [SwaggerIgnore]
    public decimal SubTotal => Items.Sum(x => x.TotalPrice);

    public string? UserName { get; set; }

    [NotMapped]
    public string? CouponCode { get; set; }

    public decimal? DiscountApplied { get; set; }

    public void SetDiscount(decimal discount)
    {
        TotalPrice = Items.Sum(x => x.TotalPrice);
        TotalPrice -= discount;
        DiscountApplied = discount;
    }

}