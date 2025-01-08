
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;
namespace Basket.Domain.Entities;

[ExcludeFromCodeCoverage]
public class CartItem
{
    [BsonElement("ProductId")]
    public string? ProductId { get; set; }

    [BsonElement("ProductName")]
    public string? ProductName { get; set; }

    [BsonElement("Quantity")]
    public int Quantity { get; set; }

    [BsonElement("UnitPrice")]
    public decimal UnitPrice { get; set; }

    [BsonElement("TotalPrice")]
    public decimal TotalPrice => Quantity * UnitPrice;
}
