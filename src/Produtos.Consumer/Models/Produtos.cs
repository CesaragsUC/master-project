using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Produtos.Consumer.Models
{
    internal class Produtos
    {
        [BsonElement("ProdutoId")]
        public string? ProdutoId { get; set; }

        [BsonElement("Nome")]
        public string? Nome { get; set; }

        [BsonElement("Preco")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Preco { get; set; }

        [BsonElement("Active")]
        public bool Active { get; set; }

        [BsonElement("ImageUri")]
        public string? ImageUri { get; set; }

        [BsonElement("CreatAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? CreatAt { get; set; }
    }
}
