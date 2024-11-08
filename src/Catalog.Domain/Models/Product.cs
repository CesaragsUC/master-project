using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Models;

public class Products
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
}
