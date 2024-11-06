namespace Domain.Models;

public class Produto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public bool Active { get; set; }
    public DateTime? CreatAt { get; set; }
}
