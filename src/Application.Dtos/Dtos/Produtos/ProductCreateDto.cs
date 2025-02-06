using System.Diagnostics.CodeAnalysis;

namespace Application.Dtos.Dtos.Produtos;

[ExcludeFromCodeCoverage]
public record ProductCreateDto
{
    public string? ProductId { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public bool Active { get; set; }
    public string? ImageUri { get; set; }
}
