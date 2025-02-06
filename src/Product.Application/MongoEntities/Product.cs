using EasyMongoNet.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Product.Application.Utils;

namespace Product.Application.MongoEntities;

[BsonCollection("Products")]
public class Products : Document
{

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

}

