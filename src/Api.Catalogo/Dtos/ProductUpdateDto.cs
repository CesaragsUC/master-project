using Api.Catalogo.Models;

namespace Api.Catalogo.Dtos;

public record ProductUpdateDto
{
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public string? ImageUri { get; set; }
    public decimal? Preco { get; set; }
    public bool Active { get; set; }

    public static implicit operator Product(ProductUpdateDto dto)
    {
        return new Product
        {
            ProductId = dto.ProductId,
            Name = dto.Name,
            ImageUri = dto.ImageUri,
            Price = dto.Preco,
            Active = dto.Active
        };
    }
}