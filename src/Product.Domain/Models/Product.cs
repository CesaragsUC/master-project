using System.ComponentModel.DataAnnotations.Schema;

namespace Product.Domain.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public bool Active { get; set; }

    public string? ImageUri { get; set; }

    public DateTime? CreatAt { get; set; }

    [NotMapped]
    public string? ProductId { get; set; }
}
