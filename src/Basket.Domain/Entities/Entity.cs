using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Domain.Entities;

[ExcludeFromCodeCoverage]
public abstract class Entity
{
    [BsonId]
    public string? Id { get; set; }

    protected Entity()
    {
        Id = Guid.NewGuid().ToString();
    }
}
