using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Domain.Entities;

[ExcludeFromCodeCoverage]
public class Cart
{
    [BsonElement("CustomerId")]
    public string? CustomerId { get; set; }

    [BsonElement("Items")]
    public List<CartItem> Items { get; set; } = new();

    [BsonElement("TotalPrice")]
    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
}
