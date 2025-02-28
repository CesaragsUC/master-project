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


    public void UpdateCarItem(List<CartItem> item, Guid productId, int quantity)
    {
        var existingItem = item.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is null)
            return;

        Items.Remove(existingItem);

        existingItem.Quantity = quantity;
        existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;

        Items.Add(existingItem);

        TotalPrice = Items.Sum(x => x.TotalPrice);
    }

    public void UpdateTotalPriceCart(decimal discount)
    {
        TotalPrice -= discount;
    }

    public void RemoveItem(List<CartItem> items, Guid productId)
    {
        var existingItem = items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is null)
            return;

        Items.Remove(existingItem);

        TotalPrice = Items.Sum(x => x.TotalPrice);
    }

    public void UpdateTotalPrice(Cart cart)
    {
        TotalPrice = Items.Sum(x => x.TotalPrice);
    }
}
