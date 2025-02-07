
using EasyMongoNet.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;
namespace Basket.Domain.Entities;

[ExcludeFromCodeCoverage]
public class CartItem : Document
{
    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }

    public string? ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}
