using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Domain.Models;

namespace Domain.Events;

public class ProdutoAtualizadoEvent : IRequest<bool>
{
    [BsonElement("ProdutoId")]
    public string? ProdutoId { get; set; }

    [BsonElement("Nome")]
    public string? Nome { get; set; }

    [BsonElement("Preco")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Preco { get; set; }

    [BsonElement("Active")]
    public bool Active { get; set; }

    [BsonElement("ImageUri")]
    public string? ImageUri { get; set; }

}
