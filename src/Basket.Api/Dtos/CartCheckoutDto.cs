using Messaging.Contracts.Events.Checkout;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class CartCheckoutDto
{

    public string? CustomerId { get; set; }

    public int? Status { get; set; }

    public List<CartItensDto> Items { get; set; } = new();

    public decimal SubTotal { get; set; }

    public string? Name { get; set; }

    public decimal TotalAmount { get; set; }

    public string? PaymentToken { get; set; }

    [NotMapped]
    public string? CouponCode { get; set; } 
}
