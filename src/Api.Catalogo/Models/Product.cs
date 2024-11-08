using Api.Catalogo.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Catalogo.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }

    [BsonElement("ProductId")]
    public string? ProductId { get; set; }

    [BsonElement("Name")]
    public string? Name { get; set; }

    [BsonElement("ImageUri")]
    public string? ImageUri { get; set; }

    [BsonElement("Price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Price { get; set; }

    [BsonElement("Active")]
    public bool Active { get; set; }

    [BsonElement("CreatAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CreatAt { get; set; }

    public static implicit operator ProductDto(Product produto)
    {
        return new ProductDto
        {
            ProductId = produto.ProductId,
            Name = produto.Name,
            Price = produto.Price,
            ImageUri = produto.ImageUri,
            Active = produto.Active,
            CreatAt = produto.CreatAt
        };
    }

    public static implicit operator ProductCreateDto(Product produto)
    {
        return new ProductCreateDto
        {
            Name = produto.Name,
            Price = produto.Price,
            Active = produto.Active
        };
    }

    public static implicit operator ProductUpdateDto(Product produto)
    {
        return new ProductUpdateDto
        {
            ProductId = produto.ProductId,
            Name = produto.Name,
            Preco = produto.Price,
            Active = produto.Active,
        };
    }
}