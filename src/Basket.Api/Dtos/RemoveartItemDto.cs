using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class RemoveartItemDto
{
    public Guid CustomerId { get; set; }

    public Guid ProductId { get; set; }

}