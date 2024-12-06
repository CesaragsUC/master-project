using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MediatR;

namespace Messaging.Contracts.Events.Product;

public class ProductDeletedEvent : IRequest<bool>
{
    [BsonElement("ProductId")]
    public string? ProductId { get; set; }
}