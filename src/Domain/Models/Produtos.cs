using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class Produtos
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Nome { get; set; }

    public decimal Preco { get; set; }

    public bool Active { get; set; }

    public string? ImageUri { get; set; }

    public DateTime? CreatAt { get; set; }

    [NotMapped]
    public string? ProdutoId { get; set; }
}
