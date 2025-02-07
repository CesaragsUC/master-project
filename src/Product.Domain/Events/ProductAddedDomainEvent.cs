namespace Product.Domain.Events;

public class ProductAddedDomainEvent
{
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }
    public string? ImageUri { get; set; }
    public DateTime? CreatAt { get; set; }
}
