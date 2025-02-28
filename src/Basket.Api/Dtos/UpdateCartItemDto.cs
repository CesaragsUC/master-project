using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class UpdateCartItemDto
{
    public Guid CustomerId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

}