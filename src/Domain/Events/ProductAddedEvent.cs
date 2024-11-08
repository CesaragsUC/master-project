using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MediatR;
using Domain.Models;

namespace Domain.Events
{
    public class ProductAddedEvent : IRequest<bool>
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

        [BsonElement("CreatAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? CreatAt { get; set; }
            
    }
}
