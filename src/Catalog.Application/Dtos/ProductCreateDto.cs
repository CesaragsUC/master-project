using System.Diagnostics.CodeAnalysis;

namespace Catalog.Application.Dtos;

[ExcludeFromCodeCoverage]
public record ProductCreateDto
{
    public string? ProductId { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public bool Active { get; set; }
    public string? ImageUri { get; set; }
}
