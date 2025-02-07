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

    public decimal TotalPrice => Items.Sum(x => x.TotalPrice);

    [SwaggerIgnore]
    public decimal SubTotal { get; set; }

    public string? UserName { get; set; }

    [NotMapped]
    public string? CouponCode { get; set; }

    public void SetDiscount(decimal discount)
    {
        SubTotal = TotalPrice - discount;
    }
}
