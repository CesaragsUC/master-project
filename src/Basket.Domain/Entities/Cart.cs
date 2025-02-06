using EasyMongoNet.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Domain.Entities;

[ExcludeFromCodeCoverage]
public class Cart : Document
{
    [BsonElement("CustomerId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CustomerId { get; set; }

    [BsonElement("Items")]
    public List<CartItem> Items { get; set; } = new();

    [BsonElement("TotalPrice")]
    public decimal TotalPrice { get; set; }
}
