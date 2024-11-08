using Api.Catalogo.Models;

namespace Api.Catalogo.Dtos;

public record ProductCreateDto
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public bool Active { get; set; }
    public string? ImageUri { get; set; }

    public static implicit operator Product(ProductCreateDto dto)
    {
        return new Product
        {
            ProductId = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Price = dto.Price,
            Active = dto.Active,
            ImageUri = dto.ImageUri,
            CreatAt =  DateTime.Now
        };
    }
}
