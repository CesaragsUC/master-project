using Api.Catalogo.Models;

namespace Api.Catalogo.Dtos;

public record ProdutoUpdateDto
{
    public string? ProdutoId { get; set; }
    public string? Nome { get; set; }
    public decimal? Preco { get; set; }
    public bool Active { get; set; }

    public static implicit operator Produtos(ProdutoUpdateDto dto)
    {
        return new Produtos
        {
            ProdutoId = dto.ProdutoId,
            Nome = dto.Nome,
            Preco = dto.Preco,
            Active = dto.Active
        };
    }
}