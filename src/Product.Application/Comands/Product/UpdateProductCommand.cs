using MediatR;
using ResultNet;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Product.Application.Comands.Product;

[ExcludeFromCodeCoverage]
public class UpdateProductCommand : IRequest<Result<bool>>
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [Base64String]
    [JsonPropertyName("imageBase64")]
    public string? ImageBase64 { get; set; }
}
