using Api.Catalogo.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Catalogo.Dtos;

public record ProductDto
{
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUri { get; set; }
    public bool Active { get; set; }
    public DateTime? CreatAt { get; set; }

    public static implicit operator Product(ProductDto dto)
    {
        return new Product
        {
            ProductId = Guid.Parse(dto.ProductId!).ToString(),
            Name = dto.Name,
            Price = dto.Price,
            ImageUri = dto.ImageUri,
            Active = dto.Active,
            CreatAt = dto.CreatAt
        };
    }
}
