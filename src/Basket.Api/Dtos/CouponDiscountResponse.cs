using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountResponse
{
    [JsonPropertyName("totalDiscount")]
    public decimal TotalDiscount { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("succeed")]
    public bool Succeed { get; set; }
}
