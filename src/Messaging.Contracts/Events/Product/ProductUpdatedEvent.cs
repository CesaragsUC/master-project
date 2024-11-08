using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MediatR;

namespace Messaging.Contracts.Events.Product;

public class ProductUpdatedEvent : IRequest<bool>
{
    [BsonElement("ProductId")]
    public string? ProductId { get; set; }

    [BsonElement("Name")]
    public string? Name { get; set; }

    [BsonElement("Price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Price { get; set; }

    [BsonElement("Active")]
    public bool Active { get; set; }

    [BsonElement("ImageUri")]
    public string? ImageUri { get; set; }

}
