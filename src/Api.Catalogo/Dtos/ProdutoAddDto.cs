using Api.Catalogo.Models;

namespace Api.Catalogo.Dtos;

public record ProdutoAddDto
{

    public string? Nome { get; set; }
    public decimal? Preco { get; set; }
    public bool Active { get; set; }

    public static implicit operator Produtos(ProdutoAddDto dto)
    {
        return new Produtos
        {
            ProdutoId = Guid.NewGuid().ToString(),
            Nome = dto.Nome,
            Preco = dto.Preco,
            Active = dto.Active,
            CreatAt =  DateTime.Now
        };
    }
}
