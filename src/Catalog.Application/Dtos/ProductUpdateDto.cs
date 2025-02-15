using System.Diagnostics.CodeAnalysis;

namespace Catalog.Application.Dtos;

[ExcludeFromCodeCoverage]
public record ProductUpdateDto
{
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public string? ImageUri { get; set; }
    public decimal? Price { get; set; }
    public bool Active { get; set; }
}