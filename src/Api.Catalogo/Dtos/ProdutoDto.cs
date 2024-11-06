using Api.Catalogo.Models;

namespace Api.Catalogo.Dtos;

public record ProdutoDto
{
    public string? ProdutoId { get; set; }
    public string? Nome { get; set; }
    public decimal? Preco { get; set; } 
    public bool Active { get; set; }
    public DateTime? CreatAt { get; set; }

    public static implicit operator Produtos(ProdutoDto dto)
    {
        return new Produtos
        {
            ProdutoId = Guid.Parse(dto.ProdutoId!).ToString(),
            Nome = dto.Nome,
            Preco = dto.Preco,
            Active = dto.Active,
            CreatAt = dto.CreatAt
        };
    }
}
