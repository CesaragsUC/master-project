namespace Application.Dtos.Dtos.Produtos;

public record ProductDto
{
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUri { get; set; }
    public bool Active { get; set; }
    public DateTime? CreatAt { get; set; }
}
