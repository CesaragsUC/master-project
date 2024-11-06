using Api.Catalogo.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Catalogo.Models;

public class Produtos
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }

    [BsonElement("ProdutoId")]
    public string? ProdutoId { get; set; }

    [BsonElement("Nome")]
    public string? Nome { get; set; }

    [BsonElement("Preco")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Preco { get; set; }

    [BsonElement("Active")]
    public bool Active { get; set; }

    [BsonElement("CreatAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CreatAt { get; set; }

    public static implicit operator ProdutoDto (Produtos produto)
    {
        return new ProdutoDto
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Preco = produto.Preco,
            Active = produto.Active,
            CreatAt = produto.CreatAt
        };
    }

    public static implicit operator ProdutoAddDto(Produtos produto)
    {
        return new ProdutoAddDto
        {
            Nome = produto.Nome,
            Preco = produto.Preco,
            Active = produto.Active
        };
    }

    public static implicit operator ProdutoUpdateDto(Produtos produto)
    {
        return new ProdutoUpdateDto
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Preco = produto.Preco,
            Active = produto.Active,
        };
    }
}