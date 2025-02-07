namespace Product.Domain.Events;

public class ProductUpdatedDomainEvent
{
    public string? ProductId { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public bool Active { get; set; }

    public string? ImageUri { get; set; }
}
